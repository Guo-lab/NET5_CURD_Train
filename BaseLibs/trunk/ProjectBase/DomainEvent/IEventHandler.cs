using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ProjectBase.DomainEvent
{
    public interface IDomainEventHandler
    {
        void ConvertAndHandle(object devent);
    }
    //领域事件处理程序
    public interface IDomainEventHandler<T> : IDomainEventHandler where T : IDomainEvent
    {
        void Handle(T devent);
    }

    /// <summary>
    /// 以进程内同步方式处理领域事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DomainEventHandler<T>: IDomainEventHandler<T> where T :IDomainEvent
    {
        public abstract void Handle(T devent);
        public void ConvertAndHandle(object devent)
        {
            if (devent is not T) throw new NetArchException("框架逻辑错误");

            Handle((T)devent);
        }
    }
}
