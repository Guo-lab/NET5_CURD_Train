using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase.Promise
{
    public interface IClientPromiseManager
    {
        /// <summary>
        /// 注册或查找延迟工作。延迟工作的声明和执行在时间上异步的，通过promiseKey可以将二者关联对应起来
        /// </summary>
        /// <param name="promiseKey"></param>
        /// <returns></returns>
        ClientPromise Initiate(string promiseKey, object? clientDispatchKey);

        /// <summary>
        /// 通知客户端延迟工作已成功执行
        /// </summary>
        /// <param name="promiseKey"></param>
        /// <param name="value"></param>
        void ResolveAsync(string promiseKey, object? clientDispatchKey, object? value,Topic? topic, string? targetId=null);

        /// <summary>
        /// 通知客户端延迟工作执行异常
        /// </summary>
        /// <param name="promiseKey"></param>
        /// <param name="reason"></param>
        void RejectAsync(string promiseKey, object? clientDispatchKey, object? reason, Topic? topic, string? targetId=null);
    }
}
