using Microsoft.AspNetCore.SignalR;
using SuperSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public interface IConnectionManager
    {
        ValueTask SendToSocketTargetAsync(string targetId, object msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramObj"></param>
        /// <param name="topic"></param>
        /// <param name="targetId"></param>
        /// <param name="clientAction"></param>
        /// <returns>false表示未找到任何连接</returns>
        Task<bool> SendToSingalRTargetAsync(object paramObj, Topic? topic = null, string? targetId=null, string? clientAction=null);
        void AddSignalRConnection(string targetId, Topic topic, HubCallerContext context, IHubCallerClients clients, IGroupManager groups);

        /// <summary>
        /// 添加连接并关联到target
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="session"></param>
        void AddSocketConnection(string targetId, IAppSession session);
        void AddSocketConnection(IAppSession session);
        void RemoveSignalRConnection(string targetId);
        void RemoveSocketConnection(string targetId);

        ValueTask SendToSocketAsync(IAppSession session,string msg);
        ValueTask SendToSocketAsync(IAppSession session, RcResult rcResult);

        IList<string> GetConnectedSocketTarget();
    }

}
