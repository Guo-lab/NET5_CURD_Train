using MessageMediator.Connection;
using Newtonsoft.Json;
using ProjectBase.DomainEvent;
using ProjectBase.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MessageMediator.DomainEvent
{
    public class MQDomainEventPublisher : IDomainEventOutProcPublisher
    {
        public static readonly string KEY_EXCHANGE_PREFIX = "DomainEvent.";
        public static readonly string KEY_DLX_SUFFIX = "DLX";
        public static readonly string KEY_DEAD_LETTER_QUEUE_SUFFIX = "_DeadLetterSub";
        public static readonly string KEY_PUBLISHER_CONNECTION_PREFIX = "Publisher_";
        public IRabbitMQConnectionManager RabbitMQConnectionManager { get; set; }
        public IAlarmReporter AlarmReporter { get; set; }
        public IMessageKeeper<IDomainEvent, string> MessageKeeper { get; set; }
        public MQDomainEventDeadLetterSubscriber MQDomainEventDeadLetterSubscriber { get; set; }
        public IChannelStorage ChannelStorage { get; set; }

        private IConnection? connection;
        private string connectionName;
        private string publisherName;
        private string exchangeName;

        //未收到服务器确认的消息
        private ConcurrentDictionary<ulong, string> outstandingConfirms = new();
        private ConcurrentQueue<string> republishQueue = new();
        private DirectoryInfo republishDir;

        //定时自动重发
        private Timer republishTimerReferenceHolder;

        //定时检查未收到回调的发布
        private Timer outstandingConfirmCheckTimerReferenceHolder;

        /// <summary>
        /// 启动发布者同时会启动对自己发布的死信的订阅
        /// </summary>
        /// <param name="publisherName"></param>
        /// <param name="republishInterval"></param>
        public void Setup(string publisherName, int republishInterval = 60 * 60 * 1000)
        {
            LoadRepublishQueue();
            this.publisherName = publisherName;
            exchangeName = KEY_EXCHANGE_PREFIX + publisherName;

            connectionName = KEY_PUBLISHER_CONNECTION_PREFIX + publisherName;
            connection = RabbitMQConnectionManager.Connect(connectionName);
            if (connection == null) return;

            republishTimerReferenceHolder = new Timer((state) => Republish(), null, 0, republishInterval);
            outstandingConfirmCheckTimerReferenceHolder = new Timer((state) => CheckOutstandingConfirm(), null, 0, republishInterval);

            MQDomainEventDeadLetterSubscriber.Setup(publisherName);
        }

        public virtual bool Publish(IDomainEvent devent)
        {
            try
            {
                var messageObj = new DomainEventWrapper()
                {
                    TypeName = devent.GetType().FullName!,
                    DomainEventObj = JsonConvert.SerializeObject(devent)
                };
                var message = JsonConvert.SerializeObject(messageObj);
                PublishMessage(message);
                return true;
            }
            catch (Exception e)
            {
                MessageKeeper.KeepToPublish(devent, "发布程序异常");
                AlarmReporter.Error("DomainEvent发布异常：", e);
                return false;
            }
        }
        private void PublishMessage(string message)
        {
            var channel = ChannelStorage.GetChannel(connectionName,(chn)=> InitChannel(chn));
            lock (channel)//防止多线程同时使用同一channel
            {
                outstandingConfirms.TryAdd(channel.NextPublishSeqNo, message);

                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.BasicPublish(exchangeName, "ignored", true, properties, body);
                //以上BasicPublish必须是最后一行，以下不可再添加任何代码，以防代码异常被误认为消息未发送成功
            }
        }
        private void InitChannel(IModel channel){
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true, false);
            channel.ConfirmSelect();
            channel.BasicAcks += Channel_BasicAcks;
            channel.BasicNacks += Channel_BasicNacks;
            channel.BasicReturn += Channel_BasicReturn;
            channel.ModelShutdown += Publisher_ChannelShutdown;
        }
        private void Publisher_ChannelShutdown(object? sender, ShutdownEventArgs ea)
        {
            AlarmReporter.Warn("Publisher发生channel被关闭事件：Initiator=" + ea.Initiator
                + "\r\n code=" + ea.ReplyCode + " text=" + ea.ReplyText
                + "\r\n cause=" + JsonConvert.SerializeObject(ea.Cause));
        }
        protected void Channel_BasicReturn(object? sender, BasicReturnEventArgs ea)
        {
            //此方法被MQ线程调用，因此必须自行进行错误处理
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                MessageKeeper.KeepToPublish(message, "消息因找不到可投递的队列而被退回");
                AlarmReporter.Error(publisherName + " 消息因找不到可投递的队列而被退回: " + message);
            }
            catch (Exception e)
            {
                AlarmReporter.Error("Channel_BasicReturn运行异常: ", e);
            }
        }
        protected void Channel_BasicAcks(object? sender, BasicAckEventArgs ea)
        {
            //此方法被MQ线程调用，因此必须自行进行错误处理
            try
            {
                ClearOutstandingConfirms(ea.DeliveryTag, ea.Multiple,false);
            }
            catch (Exception e)
            {
                AlarmReporter.Error("Channel_BasicAcks运行异常: ", e);
            }
        }
        protected void Channel_BasicNacks(object? sender, BasicNackEventArgs ea)
        {
            //此方法被MQ线程调用，因此必须自行进行错误处理
            try
            {
                AlarmReporter.Error(publisherName + " 发布的消息未被mediator正确接收-Nacked:");
                ClearOutstandingConfirms(ea.DeliveryTag, ea.Multiple,true);
            }
            catch (Exception e)
            {
                AlarmReporter.Error("Channel_BasicNacks运行异常: ", e);
            }
        }
        protected void ClearOutstandingConfirms(ulong sequenceNumber, bool multiple,bool requeue)
        {
            Func<KeyValuePair<ulong, string>, bool> filter = multiple ? k => k.Key <= sequenceNumber : k => k.Key == sequenceNumber;
            var confirmed = outstandingConfirms.Where(filter);
            foreach (var entry in confirmed)
            {
                outstandingConfirms.TryRemove(entry.Key,out _);
            }
            if (!requeue) return;

            foreach (var pair in confirmed)
            {
                AddOrDeleteQueueFile(pair.Value, true);
                republishQueue.Enqueue(pair.Value);
            }
        }
        public virtual bool Support(Type domainEventClass)
        {
            return true;
        }

        //对于publisher发出但被nack的消息，自动重发
        protected void Republish()
        {
            //此方法被定时器线程调用，因此必须自行进行错误处理
            try
            {
                while (!republishQueue.IsEmpty)
                {
                    if (republishQueue.TryDequeue(out string? message))
                    {
                        AlarmReporter.Info("自动重发: " + message);
                        try
                        {
                            PublishMessage(message!);
                            AddOrDeleteQueueFile(message!,false);
                        }
                        catch (Exception e)
                        {
                            AlarmReporter.Error("重发失败: " + message);
                            AddOrDeleteQueueFile(message,true);
                            republishQueue.Enqueue(message!);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AlarmReporter.Error("Republish运行异常: ", e);
            }
        }

        //检查服务器确认，看是否有未收到回调的
        protected void CheckOutstandingConfirm()
        {
            //只在半夜执行
            if (DateTime.Now.Hour != 1 && DateTime.Now.Hour != 2) return;

            //此方法被定时器线程调用，因此必须自行进行错误处理
            try
            {
                if (!outstandingConfirms.IsEmpty)
                {
                    var msg = "";
                    foreach(var kv in outstandingConfirms)
                    {
                        MessageKeeper.KeepToPublish(kv.Value, "发送消息未得到服务器确认");
                        msg = msg + "\r\n" + kv.Value;
                    }
                    AlarmReporter.Error(publisherName + " 消息未得到服务器确认: " + msg);
                }
            }
            catch (Exception e)
            {
                AlarmReporter.Error("CheckOutstandingConfirm异常: ", e);
            }
        }

        protected void LoadRepublishQueue()
        {
            republishDir = Directory.CreateDirectory(GetRunningPath("DomainEvent_RepublishQueue"));
            foreach (var f in republishDir.GetFiles())
            {
                republishQueue.Enqueue(File.ReadAllText(f.FullName));
            }
        }
        private string GetRunningPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\" + relativePath;
        }

        private void AddOrDeleteQueueFile(string message,bool add)
        {
            var messageObj = JsonConvert.DeserializeObject<DomainEventWrapper>(message)!;
            var path = republishDir.FullName + "\\" + messageObj.EventId.ToString();
            if (add)
            {
                File.WriteAllText(path,message);
            }
            else
            {
                File.Delete(path);
            }
        }

        public void RestorePublish()
        {
            foreach (var kv in MessageKeeper.GetToPublish())
            {
                if (Publish(kv.Value))
                {
                    MessageKeeper.MarkAsDonePublish(kv.Key);
                }
            }
        }

        //public static void CloseChannel()
        //{
        //    if (channelPerThread != null && channelPerThread.Value != null)
        //    {
        //        channelPerThread.Value.Close();
        //        channelPerThread.Value = null;
        //        channelPerThread = null;
        //    }
        //}
    }
}
