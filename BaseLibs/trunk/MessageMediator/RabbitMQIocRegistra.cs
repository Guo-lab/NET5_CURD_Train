using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MessageMediator.DomainEvent;
using ProjectBase.DomainEvent;
using System;

namespace MessageMediator
{
    public class RabbitMQIocRegistra
    {
        public static void IocRegister(IWindsorContainer container)
        {
            container.Register(
                Component.For<IDomainEventOutProcSubscriber>()
                .ImplementedBy<MQDomainEventSubscriber>(),
                Component.For<IDomainEventOutProcPublisher>()
                .ImplementedBy<MQDomainEventPublisher>(),
                Component.For<MQDomainEventDeadLetterSubscriber>()
                .ImplementedBy<MQDomainEventDeadLetterSubscriber>(),
                Component.For(typeof(IMessageKeeper<IDomainEvent, string>))
                .ImplementedBy<DomainEventMessageKeeper>()
            );
        }
    }
}
