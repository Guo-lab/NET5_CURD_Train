using MassTransit;
using ProjectBase.MessageSender;
using System;

namespace Masstransit.RabbitMQ.Core
{
    public abstract class MessageSenderBase<T>:IMessageSender<T> where T:class
                                                                       
    {
        IBusControl _bus;
        Uri _sendToUri;
        private IBusControl Bus
        {
            get
            {
                if (_bus == null)
                    _bus = BusCreator.CreateBus();
                return _bus;
            }
        }

		protected MessageSenderBase(Uri sendToUrl)
		{
			this._sendToUri = sendToUrl;
		}
        		
		public async void Send(T message) 
		{
			var endPoint = await Bus.GetSendEndpoint(_sendToUri);

			await endPoint.Send(message);

		}
        public async void Send(T message, Uri sendToUri)
        {
            var endPoint = await Bus.GetSendEndpoint(sendToUri);

            await endPoint.Send(message);

        }

        public async void Publish(T message) 
        {
            await Bus.Publish(message);
        }
    }
}








