using Newtonsoft.Json;
using ProjectBase.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator.DomainEvent
{
    public interface IDomainEventWrapper
    {
        DomainEventWrap Wrap(IDomainEvent devent);
    }
    public class DomainEventWrapper: IDomainEventWrapper
    {
        public virtual DomainEventWrap Wrap(IDomainEvent devent)
        {
            return new DomainEventWrap()
            {
                TypeName = devent.GetType().FullName!,
                DomainEventObj = JsonConvert.SerializeObject(devent)
            };
        }
    }

}
