using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Masstransit.RabbitMQ.Core
{
    public static class BusCreator
    {
        public static IBusControl CreateBus(Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost> registrationAction = null)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                //todo RabbitMQ
                //var host = cfg.Host(new Uri(RabbitMqConstants.RabbitMqUri), hst =>
                //{
                //    hst.Username(RabbitMqConstants.UserName);
                //    hst.Password(RabbitMqConstants.Password);
                //});

                //if (registrationAction != null)
                //{
                //    registrationAction.Invoke(cfg, host);
                //}
            });
        }
    }
}
