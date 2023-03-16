using SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocketBase.Promise
{
    public class BaseClientPromiseManager : IClientPromiseManager
    {
        public IConnectionManager ConnectionManager { get; set; }
        public ISocketContextAccessor Hca { get; set; }

        public ClientPromise Initiate(string promiseKey,object? clientDispatchKey)
        {
            var clientPromise = new ClientPromise(promiseKey,clientDispatchKey);
            clientPromise.InitiatingACA = Hca.Request?.ACA??"";
            return clientPromise;
        }

        public void ResolveAsync(string promiseKey, object? clientDispatchKey, object? value, Topic? topic, string? targetId=null)
        {
            var clientPromise = Complete(promiseKey, clientDispatchKey);
            clientPromise.ServerReturnStatus = ClientServerConst.CLIENT_PROMISE_SERVER_RETURN_STATUS_RESOLVED;
            clientPromise.ServerResolvedValue = value;
            if (value != null)
            {
                clientPromise.ResolvedDataType = value.GetType().Name;
            }
            ConnectionManager.SendToSingalRTargetAsync(clientPromise, topic, targetId, GetCilentCallbackName());
        }
        public void RejectAsync(string promiseKey, object? clientDispatchKey, object? reason, Topic? topic, string? targetId=null)
        {
            var clientPromise = Complete(promiseKey, clientDispatchKey);
            clientPromise.ServerReturnStatus = ClientServerConst.CLIENT_PROMISE_SERVER_RETURN_STATUS_REJECTED;
            clientPromise.ServerRejectReason = reason;
            ConnectionManager.SendToSingalRTargetAsync(clientPromise, topic, targetId, GetCilentCallbackName());
        }

        private ClientPromise Complete(string promiseKey, object? clientDispatchKey)
        {
            var clientPromise = new ClientPromise(promiseKey, clientDispatchKey);
            clientPromise.CompletingACA = Hca.Request?.ACA ?? "";
            return clientPromise;
        }
        /// <summary>
        /// 指定客户端接收延迟工作执行结果的回调方法名(SignalR回调客户端方法)
        /// </summary>
        /// <returns></returns>
        protected string GetCilentCallbackName()
        {
            return ClientServerConst.CLIENT_PROMISE_MANAGER_CALL_BACK_METHOD;
        }
    }
}
