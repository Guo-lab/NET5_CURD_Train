using Castle.Windsor;
using ProjectBase.Domain.Transaction;
using ProjectBase.Utils;
using System;

namespace ProjectBase.DomainEvent
{
    public class Dispatcher:IDispatcher 
    {
        public IDomainEventOutProcPublisher? DomainEventOutProcPublisher { get; set; }
        public ITransactionHelper TransactionHelper { get; set; }
        public IWindsorContainer WindsorContainer { get; set; }
        public IUtil Util { get; set; }

        public void Publish<T>(T devent, PublishScopeEnum scope=PublishScopeEnum.Both) where T : IDomainEvent
        {
            if (scope== PublishScopeEnum.InProc|| scope == PublishScopeEnum.Both)
            {
                PublishInProc(devent);
            }
            if (DomainEventOutProcPublisher!=null 
                && (scope == PublishScopeEnum.OutProc || scope == PublishScopeEnum.Both)
                && DomainEventOutProcPublisher.Support(typeof(T)))
            {
                if (TransactionHelper.IsInTrans())
                {
                    TransactionHelper.AddSyncTask(() =>
                    {
                        DomainEventOutProcPublisher.Publish(devent);
                    });
                }
                else
                {
                    DomainEventOutProcPublisher.Publish(devent);
                }
            }
        }

        /// 查找IDomainEvent的进程内的所有处理程序，并以同步的方式逐个调用
        private void PublishInProc<T>(T events) where T :IDomainEvent
        {
            var handlers = WindsorContainer.ResolveAll<IDomainEventHandler<T>>();
            foreach(var handler in handlers)
            {
                handler.Handle(events);
                WindsorContainer.Release(handler);
            }
        }
        public void TransferOutterEventInProc(Type domainEventClass, object devent)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEventClass);
            var handlers = WindsorContainer.ResolveAll(handlerType);
            foreach (var handler in handlers)
            {
                ((IDomainEventHandler)handler).ConvertAndHandle(devent);
            }
        }
    }

}
