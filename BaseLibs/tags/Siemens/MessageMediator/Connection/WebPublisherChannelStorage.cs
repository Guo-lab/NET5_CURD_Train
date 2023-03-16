using ProjectBase.Web.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator.Connection
{
    /// <summary>
    /// 每线程独占一个channel，在线程结束时释放。此类用于web环境下释放channel。
    /// </summary>
    public class WebPublisherChannelStorage: IChannelStorage
    {
        public static readonly string KEY_CHANNELS_FOR_CURRENT_THREAD="ChannelsForCurrentThread.";
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

        public IModel GetChannel(string connectionName,Action<IModel> initChannel)
        {
            IModel channel ;
            var channels =(Dictionary<string,IModel>?) Hca.HttpContext!.Items[KEY_CHANNELS_FOR_CURRENT_THREAD];
            if (channels != null && channels.ContainsKey(connectionName))
            {
                channel= channels[connectionName];
            }
            channel= RabbitMQConnectionManager.RetrieveChannel(connectionName, initChannel)!;
            SetChannel(connectionName, channel);
            return channel;
        }
        public void SetChannel(string connectionName, IModel channel)
        {
            var channels = (Dictionary<string, IModel>?)Hca.HttpContext!.Items[KEY_CHANNELS_FOR_CURRENT_THREAD];
            if (channels == null)
            {
                channels = new Dictionary<string, IModel>();
            }
            channels[connectionName] = channel;
            Hca.HttpContext!.Items[KEY_CHANNELS_FOR_CURRENT_THREAD] = channels;
        }

        //请求结束时关闭当前线程对应的channel
        public void Application_EndRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var channels = (Dictionary<string, IModel>?)context.Items[KEY_CHANNELS_FOR_CURRENT_THREAD];
            if(channels!=null && channels.Count > 0)
            {
                foreach(var channel in channels)
                {
                    RabbitMQConnectionManager.ReleaseChannel(channel.Key, channel.Value);
                }
            }
        }
    }
}
