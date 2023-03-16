using MassTransit;
using ProjectBase.DomainEvent;

namespace Masstransit.RabbitMQ.Core
{
    //通过RabbitMQ发布领域事件
    public abstract class MessagePublishBase:IDispatcher
    {
        IBusControl _bus;
        private IBusControl Bus
        {
            get
            {
                if (_bus == null)
                    _bus = BusCreator.CreateBus();
                return _bus;
            }
        }

		
		public async void Publish<T>(T message)  where T:IDomainEvent
        {
            await Bus.Publish(message);
        }

    }
}
