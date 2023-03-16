using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator.Connection
{
    public interface IChannelStorage
    {
        IModel GetChannel(string connectionName, Action<IModel> initChannel);
        void SetChannel(string connectionName,IModel channel);
    }
  
}
