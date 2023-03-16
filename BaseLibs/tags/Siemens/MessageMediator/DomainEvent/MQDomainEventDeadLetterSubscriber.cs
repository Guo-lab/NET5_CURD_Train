using Castle.Windsor;
using MessageMediator.Connection;
using Newtonsoft.Json;
using ProjectBase.DomainEvent;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMediator.DomainEvent
{
    /// <summary>
    /// DomainEvent死信处理。
    /// 每个publisher建一个exchange和一个queque用来记录死信。
    /// </summary>
    public class MQDomainEventDeadLetterSubscriber
    {
        public IRabbitMQConnectionManager RabbitMQConnectionManager { get; set; }
        public IDomainEventOutProcSubscriber DomainEventOutProcSubscriber { get; set; }
        public IAlarmReporter AlarmReporter { get; set; }
        public IMessageKeeper<IDomainEvent, string> MessageKeeper { get; set; }
        public IWindsorContainer WindsorContainer { get; set; }

        private IConnection? connection;
        private string publisherName;

        public void Setup(string publisherName) 
        {
            this.publisherName = publisherName;

            //不知为什么DomainEventOutProcSubscriber无法通过属性依赖获得，（检查了也没有循环依赖），似乎是作为可选依赖不知何故被容器决定给个空值
            //如果通过构建器声明为必有依赖就能得到，但需要依赖链上都用构建器声明为必有依赖。
            //此处为简化，采用手动解析。
            DomainEventOutProcSubscriber = WindsorContainer.Resolve<IDomainEventOutProcSubscriber>();

            if (DomainEventOutProcSubscriber?.ConnectionName != null)
            {
                connection = RabbitMQConnectionManager.GetConnection(DomainEventOutProcSubscriber.ConnectionName);
            }
            else
            {
                connection = RabbitMQConnectionManager.Connect(MQDomainEventSubscriber.KEY_SUBSCRIBER_CONNECTION_PREFIX+publisherName);
            }

            try
            {
                Subscribe();
            }
            catch (Exception e)
            {
                AlarmReporter.Error("死信订阅异常：", e);
            }
        }

        private void Subscribe()
        {
            string exchangeName = MQDomainEventPublisher.KEY_EXCHANGE_PREFIX + publisherName + MQDomainEventPublisher.KEY_DLX_SUFFIX;
            string queueName = exchangeName + MQDomainEventPublisher.KEY_DEAD_LETTER_QUEUE_SUFFIX;
            var channel = connection!.CreateModel();
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true, false);
            channel.QueueBind(queueName, exchangeName, "ignored");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += DeadLetter_Received;
            channel.BasicConsume(queueName, false, consumer);
        }

        private void DeadLetter_Received(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            //此方法被MQ线程调用，因此必须自行进行错误处理
            try
            {
                var reason = ea.BasicProperties.Headers["x-death"];
                var reasonMsg ="死因: "+( reason == null ? "" : JsonConvert.SerializeObject(reason))+"\r\n";
                MessageKeeper.KeepToPublish(reasonMsg+message, "死信");
                AlarmReporter.Error("接收到死信：" + message);
                ((EventingBasicConsumer)sender!).Model.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                AlarmReporter.Error("死信处理时发生错误："+ message, e);
                throw;
            }
        }

    }

}
