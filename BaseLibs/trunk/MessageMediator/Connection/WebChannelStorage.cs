using ProjectBase.Web.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageMediator.Connection
{
    /// <summary>
    /// 每线程独占一个channel，在线程结束时释放。此类用于web环境下释放channel。
    /// </summary>
    public class WebChannelStorage: IChannelStorage
    {
        protected const string HttpContextChannelStorageKey = "HttpContextChannelStorageKey";

        private static ThreadLocal<SimpleChannelStorage> storageForTaskThread;

        public Microsoft.AspNetCore.Http.IHttpContextAccessor Hca { get; set; }
        public IRabbitMQConnectionManager RabbitMQConnectionManager { get; set; }

        private RequestListenerRegistra registra;
        public RequestListenerRegistra Registra
        {
            get
            {
                return registra;
            }
            set
            {
                registra = value;
                registra.AddEndHandler(Application_EndRequest);
            }
        }

        public IModel? GetChannel(string connectionName)
        {
            return GetSimpleChannelStorage().GetChannel(connectionName);
        }
        public void SetChannel(string connectionName, IModel channel)
        {
            GetSimpleChannelStorage().SetChannel(connectionName, channel);
        }

        public virtual SimpleChannelStorage GetSimpleChannelStorage()
        {
            var context = Hca.HttpContext;
            if (context != null)
            { //有request从request里取. 由于异步线程的时机可能在request结束之前，此时可能是与request同一线程还是能访问到HttpContext，所以需要异步线程启动时给个标志，此处通过这个标志判断异步任务不使用HttpContext
                var storage = (SimpleChannelStorage?)context.Items[HttpContextChannelStorageKey];
                if (storage == null)
                {
                    storage = GetNewStorage();
                    context.Items[HttpContextChannelStorageKey] = storage;
                }
                return storage;
            }
            else
            {//没有request就把channelstorage存在当前线程里
                if (storageForTaskThread == null)
                {
                    storageForTaskThread = GetInitValueOfStorageForTaskThread();
                }
                return storageForTaskThread.Value!;
            }
        }
        protected virtual ThreadLocal<SimpleChannelStorage> GetInitValueOfStorageForTaskThread()
        {
            return new ThreadLocal<SimpleChannelStorage>(() => new SimpleChannelStorage());
        }
        protected virtual SimpleChannelStorage GetNewStorage()
        {
            return new SimpleChannelStorage();
        }

        public IDictionary<string, IModel> GetAllChannels()
        {
            return GetSimpleChannelStorage().GetAllChannels();
        }

        //请求结束时关闭当前线程对应的channel
        public void Application_EndRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var storage = (SimpleChannelStorage?)context.Items[HttpContextChannelStorageKey];
            if(storage != null)
            {
                foreach(var channel in storage.GetAllChannels())
                {
                    RabbitMQConnectionManager.ReleaseChannel(channel.Key, channel.Value);
                }
            }
            context.Items.Remove(HttpContextChannelStorageKey);
        }

    }
}
