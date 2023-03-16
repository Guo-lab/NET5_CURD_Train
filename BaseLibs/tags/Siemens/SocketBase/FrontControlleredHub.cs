using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ProjectBase.BusinessDelegate;
using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public interface IFrontControlleredHub
    {
        object? HandleSignalRRequest(SignalRRequestParam param);
    }
    /// <summary>
    /// SingnalR请求统一入口
    /// </summary>
    public class FrontControlleredHub : Hub, IFrontControlleredHub
    {
        private static string KEY_TOPIC_NAME = "SignalR_TopicName";
        private static string KEY_TOPIC_VALUE = "SignalR_TopicValue";
        public IConnectionManager ConnectionManager { get; set; }
        public IApplicationStorage AppStorage { get; set; }
        public IUtil Util { get; set; }
        public FrontControlleredHub(IConnectionManager connectionManager, IApplicationStorage appStorage,IUtil util)//Hub类无法使用winsdor的属性注册，只能构建器注入了
        {
            ConnectionManager = connectionManager;
            AppStorage = appStorage;
            Util = util;
        }
        public object? HandleSignalRRequest(SignalRRequestParam param)
        {
            if (AppStorage.GetAppSetting("LogSignalR").ToLower() == "true")
            {
                Util.AddLog("SignalR 调用服务器方法：" + JsonConvert.SerializeObject(param));
            }
            return FrontController.Instance.HandleSignalRRequest(Context, param);
        }
        public override async Task OnConnectedAsync()
        {
            var topic = GetTopic();
            ConnectionManager.AddSignalRConnection(GetTargetId(), topic, Context, Clients, Groups);
            LogConnection(true, topic);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectionManager.RemoveSignalRConnection(GetTargetId());
            LogConnection(false, GetTopic());
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 获取目标身份。此处缺省实现是用连接作为目标身份。子类若改写，可以将多个连接对应到同一目标身份，或同一连接对应到不同的目标身份。
        /// 注意映射逻辑应与ConnectionManager.MapSignalRConnectionIds一致。
        /// </summary>
        /// <returns></returns>
        protected string GetTargetId()
        {
            return Context.ConnectionId;
        }
        protected Topic GetTopic()
        {
            return new Topic
            {
                Name = Context.GetHttpContext().Request.Query[KEY_TOPIC_NAME],
                Value = Context.GetHttpContext().Request.Query[KEY_TOPIC_VALUE]
            };
        }

        private void LogConnection(bool success,Topic topic)
        {
            if (AppStorage.GetAppSetting("LogSignalR").ToLower() == "true")
            {
                var msg = success ? "SignalR启动连接：" : "SignalR断开连接: ";
                Util.AddLog(msg + Context.GetHttpContext().Request.Path + "?" + JsonConvert.SerializeObject(topic));
            }
        }
    }

}
