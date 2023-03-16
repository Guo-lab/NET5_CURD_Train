using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESC5.Domain.Base.VD;

namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public abstract class ASNBase : VersionedDomainObjectWithAssignedGuidId
    {
        public virtual string ASNNo { get; set; }
        public virtual IVendor Vendor { get; set; }
        public virtual string DeliveryNo { get; set; }
        public virtual DateTime DeliveryDate { get; set; }
        public virtual DateTime? SyncTime { get; set; }
    }

    [MappingIgnore]
    public abstract partial class ASNBase<ItemT> : ASNBase where ItemT : ASNItemBase
    {
        public virtual IList<ItemT> Items { get; set; }
        public abstract void AddItem(ItemT item);       
    }
}
