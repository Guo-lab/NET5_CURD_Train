using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ProjectBase.DomainEvent
{
    //领域事件处理程序
    public interface IDomainEventHandler<T> where T : IDomainEvent
    {
        void Handle(T events);
    }

    //以进程内同步方式处理领域事件
    public abstract class DomainEventHandler<T>: IDomainEventHandler<T> where T :IDomainEvent
    {
        public abstract void Handle(T events);
    }
}
