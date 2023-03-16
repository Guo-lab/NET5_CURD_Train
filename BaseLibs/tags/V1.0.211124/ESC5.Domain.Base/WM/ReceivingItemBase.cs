using ESC5.ValueObject;
using ProjectBase.Domain;
using System;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public abstract class ReceivingItemBase : BaseDomainObject
    {
        #region "persistent properties"        
		public virtual string? LotNo { get; set; }
        public virtual UnitVO Unit { get; set; }
		public virtual decimal ReceivedQuantity { get; set; }
		public virtual string? Remark { get; set; }
        #endregion

    }

}

