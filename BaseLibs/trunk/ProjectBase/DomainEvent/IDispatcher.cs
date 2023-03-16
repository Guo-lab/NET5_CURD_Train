using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.DomainEvent
{
    public interface IDispatcher
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="devent"></param>
        /// <param name="scope">指定发布范围</param>
        void Publish<T>(T devent, PublishScopeEnum scope = PublishScopeEnum.Both) where T :IDomainEvent;

        /// <summary>
        /// 将接收到的进程外事件在进程内转发
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="events"></param>
        void TransferOutterEventInProc(Type domainEventClass, object devent);

    }
    public enum PublishScopeEnum
    {
        InProc = 1,
        OutProc = 2,
        Both = 3
    }
}
