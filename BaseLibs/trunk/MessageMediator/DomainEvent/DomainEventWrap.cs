using Newtonsoft.Json;
using ProjectBase.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator.DomainEvent
{
     public class DomainEventWrap
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public string TypeName { get; set; }
        public string DomainEventObj { get; set; }

    }
}
