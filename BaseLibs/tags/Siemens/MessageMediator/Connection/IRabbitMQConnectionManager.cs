using ProjectBase.Domain;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator.Connection
{
    public interface IRabbitMQConnectionManager:IBusinessDelegate
    {
        IConnection? Connect(string publisherName);
        void CloseAllConnections();
        IModel? RetrieveChannel(string connectionName, Action<IModel> initChannel);
        void ReleaseChannel(string connectionName, IModel channel);
        IConnection? GetConnection(string connectionName);
    }
}
