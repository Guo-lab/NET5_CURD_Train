using ProjectBase.Application;
namespace ProjectBase.DomainEvent
{
    public class Dispatcher:IDispatcher 
    {
        public Dispatcher()
        {
           
        }

        //查找IDomainEvent的所有处理程序，并以同步的方式逐个调用
        public void Publish<T>(T events) where T :IDomainEvent
        {
            var handlers = CastleContainer.WindsorContainer.ResolveAll<IDomainEventHandler<T>>();
            foreach(var handler in handlers)
            {
                handler.Handle(events);
                CastleContainer.WindsorContainer.Release(handler);
            }
        }
    }
}
