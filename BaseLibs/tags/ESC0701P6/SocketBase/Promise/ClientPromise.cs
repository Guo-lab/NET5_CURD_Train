using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase.Promise
{
    /// <summary>
    /// 将服务器延迟任务信息传回客户端
    /// </summary>
    public class ClientPromise
    {
        public ClientPromise(string uniqueKey,object? clientDispatchKey)
        {
            Key = uniqueKey;
            ClientDispatchKey = clientDispatchKey;
        }
        public bool IsClientPromise { get; }= true;

        /// <summary>
        /// 用于唯一识别的key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 用于客户端进行再分发的键值。类型应为可以被客户端识别的类型。
        /// 这个属性的意义是当一个ClientPromise对象返回客户端后需要由客户端根据这个属性值进一步筛选对应目标。
        /// </summary>
        public object? ClientDispatchKey { get; set; }
        

        /// <summary>
        /// 创建Promise的ACA
        /// </summary>
        public string? InitiatingACA { get; set; }

        /// <summary>
        /// 完成Promise的ACA
        /// </summary>
        public string? CompletingACA { get; set; }

        /// <summary>
        /// ServerResolvedValue的类型名
        /// </summary>
        public string? ResolvedDataType { get; set; }

        public string? ServerReturnStatus { get; set; }
        public object? ServerResolvedValue { get; set; }
        public object? ServerRejectReason { get; set; }

    }

    /// <summary>
    /// 将多个服务器延迟任务信息传回客户端
    /// </summary>
    public class ClientPromiseArray
    {
        public bool IsClientPromiseArray { get; } = true;
        public ClientPromise[] ClientPromises { get; set; }

        public ClientPromiseArray(ClientPromise[] clientPromises)
        {
            ClientPromises = clientPromises;
        }
        public ClientPromiseArray(ClientPromise clientPromise1, ClientPromise clientPromise2)
        {
            ClientPromises = new ClientPromise[] { clientPromise1 , clientPromise2 };
        }
    }
}
