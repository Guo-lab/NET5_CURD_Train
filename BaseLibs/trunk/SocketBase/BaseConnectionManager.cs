using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ProjectBase.BusinessDelegate;
using ProjectBase.Utils;
using SuperSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public abstract class BaseConnectionManager : IConnectionManager
    {
        public static string KEY_SIGNALR_TOPIC = "SocketBase.SignalRTopic";
        public static byte[] OutTextPackageTerminator { get; set; }

        public IApplicationStorage AppStorage { get; set; }
        public IUtil Util { get; set; }

        protected IDictionary<string, IAppSession> socketTargets = new ConcurrentDictionary<string,IAppSession>();
        protected IDictionary<string, IHubCallerClients> signalRConnections = new ConcurrentDictionary<string, IHubCallerClients>();//string 是connectionId
        protected IDictionary<string, Topic> targetTopics = new ConcurrentDictionary<string, Topic>();//string是targetId
        public virtual ValueTask SendToSocketTargetAsync(string targetId, object msg)
        {
            if (!socketTargets.ContainsKey(targetId)) throw new ConnectionNotFoundException();
            return SendToSocketAsync(socketTargets[targetId],msg.ToString()!);
        }
        public virtual Task<bool> SendToSingalRTargetAsync(object paramObj, Topic? topic=null, string? targetId=null, string? clientAction=null)
        {
            if (clientAction == null)
            {
                clientAction = paramObj is ServerErrorToClient ? ClientServerConst.CLIENT_CALLBACK_ON_SERVER_ERROR:ClientServerConst.CLIENT_CALLBACK_ON_SERVER_RETURN;
            }
            if (paramObj is ServerErrorToClient setc)
            {
                paramObj = setc.Reason;
            }
            IList<string> connectionIds= new List<string>();
            if (targetId != null)
            {
                connectionIds = MapSignalRConnectionIds(targetId);
            }
            else if(topic!=null)
            {
                connectionIds=targetTopics.Where(o => o.Value.Name == topic.Name && o.Value.Value == topic.Value).Select(o => o.Key).ToList();
            }
            else
            {
                connectionIds = signalRConnections.Keys.ToList();
            }
            if(connectionIds.Count==0) return Task.FromResult(false);
            foreach (var id in connectionIds)
            {
                var clients = signalRConnections[id];
                var client = clients.Client(id);
                if (client == null)
                {
                    throw new Exception("client is null");
                }
                else 
                {
                    client.SendAsync(clientAction, paramObj);
                    if (AppStorage.GetAppSetting("LogSignalR").ToLower() == "true")
                    {
                        Util.AddLog("SignalR 调用客户端方法：clientAction=" + clientAction+ " paramObj=" +JsonConvert.SerializeObject(paramObj));
                    }

                }
            }
            return Task.FromResult(true);
        }
        public virtual void AddSignalRConnection(string targetId, Topic topic,HubCallerContext context,IHubCallerClients clients,IGroupManager groups)
        {
            targetTopics[targetId] = topic;
            context.Items[KEY_SIGNALR_TOPIC] = topic;
            var connectionIds = MapSignalRConnectionIds(targetId);
            foreach(var id in connectionIds)
            {
                signalRConnections[id] = clients;
            }
        }
        public virtual void RemoveSignalRConnection(string targetId)
        {
            targetTopics.Remove(targetId);
            foreach (var connectionId in MapSignalRConnectionIds(targetId))
            {
                signalRConnections.Remove(connectionId);
            }
        }

        public virtual void AddSocketConnection(string targetId, IAppSession session)
        {
            socketTargets[targetId] = session;
        }

        /// <summary>
        /// 添加连接。子类必须实现此方法，因为父类不设置任何缺省逻辑来关联target和session。
        /// 子类应在此方法中关联target和session。
        /// </summary>
        /// <param name="session"></param>
        public abstract void AddSocketConnection(IAppSession session);
        public virtual void RemoveSocketConnection(string targetId)
        {
            CloseSocketConnection(targetId);
            socketTargets.Remove(targetId);
        }

        private void CloseSocketConnection(string targetId)
        {
            if (targetId!=null && socketTargets.ContainsKey(targetId))
            {
                var existingSession = socketTargets[targetId];
                if (existingSession.State != SessionState.Closed)
                {
                    existingSession.CloseAsync(SuperSocket.Channel.CloseReason.LocalClosing);
                }
            }
        }

        /// <summary>
        /// 将targetId对应到connectionId。此处缺省实现是tagetId就是connectionId。子类可以自定义映射逻辑，注意映射逻辑应与FrontControlleredHub.GetTargetId一致
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        protected virtual string[] MapSignalRConnectionIds(string targetId)
        {
            return new[] { targetId };
        }

        public virtual ValueTask SendToSocketAsync(IAppSession session, string msg)
        {
            if (session.State != SessionState.Connected) throw new SocketNotConnectedException();
            return session.SendAsync(Encoding.UTF8.GetBytes(msg).Concat(OutTextPackageTerminator).ToArray());
        }
        public virtual ValueTask SendToSocketAsync(IAppSession session, RcResult rcResult)
        {
            return SendToSocketAsync(session,rcResult.data?.ToString()??"");
        }

        public virtual IList<string> GetConnectedSocketTarget()
        {
            return socketTargets.Keys.ToList();
        }
    }

    public class ConnectionNotFoundException:Exception
    {

    }
    public class SocketNotConnectedException : Exception
    {

    }
    public class ServerErrorToClient
    {
        public object Reason { get; private set; }
        public ServerErrorToClient(object clientCallbackReasonParamObj)
        {
            Reason = clientCallbackReasonParamObj;
        }
    }
}
