using System;
using ESC5.ValueObject;
using ProjectBase.Domain;
using ProjectBase.Utils;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public class InboundOrderItemBase : BaseDomainObject
    {
        #region "persistent properties"
        public virtual int ItemNo { get; set; }
        public virtual string PartNo { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual UnitVO Unit { get; set; }

        #endregion
    }

}

