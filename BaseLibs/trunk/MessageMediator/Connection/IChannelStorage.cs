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
        IDictionary<string, IModel> GetAllChannels();
        IModel? GetChannel(string connectionName);
        void SetChannel(string connectionName,IModel channel);
    }
  
}
