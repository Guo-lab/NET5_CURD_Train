using ESC5.Domain.Base.IM;
using ESC5.Domain.Base.VD;
using ProjectBase.Domain;
using System;
using ESC5.ValueObject;

namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public abstract class SettlementNoticeBase : BaseDomainObject
    {
        public virtual DateTime? SettlementDateFrom { get; set; }
        public virtual DateTime? SettlementDateTo { get; set; }
        public virtual decimal SettlementPrice { get; set; }
        public virtual UnitVO Unit { get; set; }
    }

    [MappingIgnore]
    public class SettlementNoticeBase<PartT, VendorT> : SettlementNoticeBase where PartT : PartBase
                                                                                                       where VendorT : VendorBase
    {
        public virtual PartT Part { get; set; }
        public virtual VendorT Vendor { get; set; }

    }
}
