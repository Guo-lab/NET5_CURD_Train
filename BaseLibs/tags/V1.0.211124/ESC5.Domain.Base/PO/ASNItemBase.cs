using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESC5.Domain.Base.VD;
using ESC5.ValueObject;

namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public abstract class ASNItemBase : VersionedDomainObjectWithAssignedGuidId
    {
        public virtual int ASNItemNo { get; set; }
        public virtual string LotNo { get; set; }
        public virtual decimal DeliveredQuantity { get; set; }
        public virtual UnitVO Unit { get; set; }
    }
}
