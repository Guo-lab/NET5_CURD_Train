using MessageMediator.Connection;
using Newtonsoft.Json;
using ProjectBase.DomainEvent;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharpArch.Domain;
using SharpArch.NHibernate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageMediator.DomainEvent
{
    public class MQDomainEventSubscriber: IDomainEventOutProcSubscriber
    {
        public static readonly string KEY_SUBSCRIBER_CONNECTION_PREFIX = "Subscriber_";

        /// <summary>
        /// 一次可接收多少条消息。因为一次接收的消息存在MQ客户端的缓存中，因此对于本方案采用的一个队列一个consumer的场景，此值与客户端可用的缓存大小相关。
        /// </summary>
        public static ushort PrefetchCount { get; set; } = 20;
        public static int ClearInterval { get; set; } = 60 * 60 * 1000;//每小时;
        public static int TimerDelay { get; set; } = 30000;
        public IRabbitMQConnectionManager RabbitMQConnectionManager { get; set; }
        public IDispatcher InProcDispatcher { get; set; }
        public IAlarmReporter AlarmReporter { get; set; }
        public IMessageKeeper<IDomainEvent, string> MessageKeeper { get; set; }

        public string? ConnectionName { get; set; }

        //近期处理过的消息，用于防止重复消息处理
        private ConcurrentDictionary<Guid, DateTime> recentMsgs=new ();

        private IConnection? connection;
        private string subscriberName;
        private string domainEventNS;

        private Timer clearAckTimerReferenceHolder;

        public void Init(string subscriberName, string publisherNames, string domainEventNS) 
        {
            try
            {
                //<清空实例变量
                if (clearAckTimerReferenceHolder != null)
            {
                clearAckTimerReferenceHolder.Dispose();
            }
            //>

            this.subscriberName = subscriberName;
            this.domainEventNS = domainEventNS;
            ConnectionName = KEY_SUBSCRIBER_CONNECTION_PREFIX + subscriberName;
            connection = RabbitMQConnectionManager.Connect(ConnectionName);
            if (connection == null) return;

                Subscribe(publisherNames);
            }
            catch(Exception e)
            {
                AlarmReporter.Error("MQDomainEventSubscriber启动异常：", e);
            }
            clearAckTimerReferenceHolder = new Timer((state) => ClearRecentMsgInProcCache(), null, TimerDelay, ClearInterval);
        }

        private void Subscribe(string publisherNames)
        {
            foreach (var publisherName in publisherNames.Split(','))
            {
                string exchangeName = MQDomainEventPublisher.KEY_EXCHANGE_PREFIX + publisherName;
                string queueName = exchangeName + "_" + subscriberName;
                var channel = connection!.CreateModel();//一个监听订阅线程对应一个channel
                IDictionary<string, object> args = new Dictionary<string, object>();
               // args.Add("x-queue-type", "quorum");
                args.Add("x-dead-letter-exchange", exchangeName+ MQDomainEventPublisher.KEY_DLX_SUFFIX);
                channel.QueueDeclare(queueName, true, false, false, args);
                channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true, false);
                channel.QueueBind(queueName, exchangeName, "ignored");
                channel.BasicQos(0, PrefetchCount, false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                consumer.Shutdown += Consumer_ChannelShutdown;
                //exclusive:true 一个consumer对队列独占使用
                channel.BasicConsume(queue:queueName,autoAck:false, consumer: consumer, exclusive:true);
            }
        }
        private void Consumer_Received(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Dispatch(message);
            //所有接收到的无论是否正确处理都给服务器ack，以便从服务器删除消息。服务器未能收到ack的 消息通过TTL变成死信。
            ((EventingBasicConsumer)sender!).Model.BasicAck(ea.DeliveryTag, false);
        }
        private void Consumer_ChannelShutdown(object? sender, ShutdownEventArgs ea)
        {
            AlarmReporter.Warn("Subscriber发生channel被关闭事件：Initiator=" + ea.Initiator 
                + "\r\n code="+ea.ReplyCode + " text=" + ea.ReplyText 
                + "\r\n cause=" +JsonConvert.SerializeObject(ea.Cause));
        }
        private void Dispatch(string message)
        {
            //此方法被MQ线程调用，因此必须自行进行错误处理
            //进程外事件处理需手动处理类Action的上下文(进程内事件处理是在Action中进行）
            //一个EventHandler不会既处理进程内事件又处理进程外事件，处理进程外事件的Handle方法需要在类Action执行的上下文中执行。
            //此处与Action的标记事务不同，这里没有事务上下文，进程外事件处理程序需注意自定义事务(DoIntrans)
            //此方法按每条消息(请求)一个线程被调用。

            Type? domainEventClass = null;
            try
            {
                var messageObj = JsonConvert.DeserializeObject<DomainEventWrap>(message)!;
                if (!IsDuplicate(messageObj))
                {
                    domainEventClass = GetDomainEventCLass(messageObj.TypeName);
                    var devent = JsonConvert.DeserializeObject(messageObj.DomainEventObj, domainEventClass);
                    InProcDispatcher.TransferOutterEventInProc(domainEventClass, devent);

                    AddRecentMsg(messageObj);
                }
                else
                {
                    AlarmReporter.Info(subscriberName + " 重复接收(忽略) --- " + message);
                }
            }
            catch (Exception e)//类似Action执行的错误处理
            {
                MessageKeeper.KeepToHandle(message);
                AlarmReporter.Error("进程外事件处理时发生错误：" + (domainEventClass?.FullName?? "domainEventClass=null") + "\r\nmessage=" + message, e);
            }
            finally
            {
                //http请求最后会关hbsession并删storage(以便下次新建),MQ线程也须如此。
                NHibernateSession.CloseAllSessionsAndClearStorage();
            }
        }

        private Type GetDomainEventCLass(string className)
        {
            Type? domainEventClass = null;
            string[] list = domainEventNS.Split(',');
            foreach (var asm in list.Select(x => Assembly.LoadFrom(GetRunningPath(x + ".dll"))))
            {
                domainEventClass = asm.GetType(className);
                if (domainEventClass == null) continue;
            }
            Check.Require(domainEventClass != null, "未找到DomainEvent定义："+ className);
            return domainEventClass!;
        }
        private string GetRunningPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\" + relativePath;
        }

        private bool IsDuplicate(DomainEventWrap wrapper)
        {
            return recentMsgs.ContainsKey(wrapper.EventId);
        }

        private void AddRecentMsg(DomainEventWrap wrapper)
        {
            recentMsgs.TryAdd(wrapper.EventId,DateTime.Now);
        }

        private void ClearRecentMsgInProcCache()
        {
            foreach(var entry in recentMsgs)
            {
                if (entry.Value.AddHours(1) < DateTime.Now)//只留最近1小时的
                {
                    recentMsgs.TryRemove(entry);
                }
            }
         }

        public void RestoreHandle()
        {
            var all = MessageKeeper.GetToHandle();
            foreach (var kv in all)
            {
                Dispatch(kv.Value);
                MessageKeeper.MarkAsDoneHandle(kv.Key);
            }
        }

        #region TestSub
        public void TestStub_ClearRecentMsgInProcCache()
        {
            recentMsgs.Clear();
        }
        #endregion
    }

}
