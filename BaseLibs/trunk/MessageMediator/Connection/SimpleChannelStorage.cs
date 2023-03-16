using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator.Connection
{
    public class SimpleChannelStorage : IChannelStorage
    {
        private readonly Dictionary<string, IModel> storage = new();

        public IModel? GetChannel(string connectionName)
        {
            if (storage.ContainsKey(connectionName))
            {
                return storage[connectionName];
            }
            return null;
        }

        public void SetChannel(string connectionName, IModel channel)
        {
            storage[connectionName] = channel;
        }

        public IDictionary<string, IModel> GetAllChannels()
        {
            return storage;
        }

    }
}
