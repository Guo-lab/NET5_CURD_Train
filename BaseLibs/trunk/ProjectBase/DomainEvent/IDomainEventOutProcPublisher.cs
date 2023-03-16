using System;

namespace ProjectBase.DomainEvent
{
    public interface IDomainEventOutProcPublisher
    {
        void Init(string publisherName);
        /// <summary>
        /// 向进程外发布事件
        /// </summary>
        /// <param name="devent"></param>
        /// <returns>是否成功发布</returns>
        bool Publish(IDomainEvent devent);

        /// <summary>
        /// 是否支持某类事件
        /// </summary>
        /// <param name="domainEventClass"></param>
        /// <returns></returns>
        bool Support(Type domainEventClass);

        /// <summary>
        /// 发布失败的事件恢复发布
        /// </summary>
        void RestorePublish();
    }
}
