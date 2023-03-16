using ProjectBase.BusinessDelegate;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MessageMediator.Connection
{
    /// <summary>
    /// 管理RabbitMQ的连接（包括channel）
    /// </summary>
    public class RabbitMQConnectionManager: IRabbitMQConnectionManager
    {
        /// <summary>
        /// 启动连接尝试次数
        /// </summary>
        public static int RetryMax { get; set; } = 3;

        public static int ChannelPoolSize { get; set; } = 50;
        public static int ChannelPoolTimeout { get; set; } = 5000;

        public IAlarmReporter AlarmReporter { get; set; }
        public IApplicationStorage ApplicationStorage { get; set; }

        private IDictionary<string,IConnection> conns = new Dictionary<string,IConnection>();

        //每连接一个channel池，标记每个channel是否可用：false表示已被占用
        private IDictionary<string,Dictionary<IModel,bool>> channelPool = new Dictionary<string, Dictionary<IModel, bool>>();
        //多线程共享线程池，需要加锁访问
        private static string channelPoolLock = "channelPoolLock";
        public IConnection? GetConnection(string connectionName)
        {
            return conns.ContainsKey(connectionName) ? conns[connectionName] : null;
        }

        public IConnection? Connect(string connectionName)
        {
            var connectionFactory = new ConnectionFactory() { 
                HostName = ApplicationStorage.GetAppSetting("MQConnection:HostName"),
                Port = int.Parse(ApplicationStorage.GetAppSetting("MQConnection:Port")),
                UserName= ApplicationStorage.GetAppSetting("MQConnection:UserName"),
                Password = ApplicationStorage.GetAppSetting("MQConnection:Password")
            };
            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

            IConnection? connection = null;
            Exception? connException=null;
            int i = 0;
            while(connection == null && i <= RetryMax)
            {
                try
                {
                    connection = connectionFactory.CreateConnection(connectionName);
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
                {
                    connException = e;
                }
                Thread.Sleep(2000);
                i++;
            }
            if (connection != null)
            {
                connection.ConnectionBlocked += HandleBlocked;
                connection.ConnectionUnblocked += HandleUnblocked;
                conns.Add(connectionName, connection);
                channelPool.Add(connectionName, new Dictionary<IModel, bool>());
            }
            else
            {
                AlarmReporter.Error("启动连接RabbitMQ失败", connException);
            }
            return connection;
        }
   
        public void HandleBlocked(object? sender, ConnectionBlockedEventArgs args)
        {
            AlarmReporter.Info("连接受阻");
        }

        public void HandleUnblocked(object? sender, EventArgs args)
        {
            AlarmReporter.Info("连接受阻情况已恢复");
        }

        public void CloseAllConnections()
        {
            foreach(var conn in conns)
            {
                conn.Value.Close();
            }
        }

        public IModel RetrieveChannel(string connectionName, Action<IModel> initChannel)
        {
            lock (channelPoolLock)
            {
                return RetrieveChannel(connectionName, initChannel, 0);
            }
        }
        private IModel RetrieveChannel(string connectionName, Action<IModel> initChannel,int timeoutMilisec=0)
        {
            var channels=channelPool[connectionName];
            //channel已关闭则去掉
            foreach (var kv in channels.Where(o=>o.Key.IsClosed))
            {
                channels.Remove(kv.Key);
            }
            var channel = channels.FirstOrDefault(o => o.Value == true).Key;
            if (channel==null && channels.Count<ChannelPoolSize)//无可用的且未满则新建
            {
                channel = CreateChannel(connectionName, initChannel);
                channels.Add(channel, false);
            }
            else if(channel!=null && channel.IsOpen)//有可用的则占用
            {
                channels[channel]=false;
            }
            else
            {
                if (timeoutMilisec == 0)
                {
                    AlarmReporter.Info("channel池已满，获取channel需等待");
                }
                if (timeoutMilisec > ChannelPoolTimeout)
                {
                    AlarmReporter.Warn("因channel池占满使得线程等待超时,线程池超量扩充为 "+ (channels.Count+1).ToString());
                    channel = CreateChannel(connectionName, initChannel);
                    channels.Add(channel, false);
                    return channel;
                }
                else
                {
                    Thread.Sleep(100);
                    timeoutMilisec += 100;
                }
                return RetrieveChannel(connectionName, initChannel,timeoutMilisec);
            }
            return channel;
        }
        private IModel CreateChannel(string connectionName, Action<IModel> initChannel)
        {
            var channel = GetConnection(connectionName)!.CreateModel();
            initChannel.Invoke(channel);
            return channel;
        }
        public void ReleaseChannel(string connectionName,IModel channel)
        {
            lock (channelPoolLock)
            {
                var channels = channelPool[connectionName];
                channels[channel] = true;
            }
        }

        public void ClearChannelPool(string connectionName)
        {
            foreach(var chn in channelPool[connectionName].Keys)
            {
                if (chn.IsOpen)
                {
                    chn.Close();
                }
            }
            channelPool[connectionName].Clear();
        }
        public IDictionary<IModel, bool> GetChannels(string connectionName)
        {
            return channelPool[connectionName];
        }
    }
}
