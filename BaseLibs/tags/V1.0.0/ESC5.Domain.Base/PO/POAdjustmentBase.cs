using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectBase.Domain;

namespace ESC5.Domain.Base.PO
{
    public class POAdjustmentBase : BaseDomainObject
    {
        public virtual string Description { get; set; }
        public virtual decimal AdjustedAmount { get; set; }
        public virtual decimal AdjustedAmountVAT { get; set; }
        public virtual DateTime? SyncTime { get; set; }
    }
    [MappingIgnore]
    public class POAdjustmentBase<T>:POAdjustmentBase where T:PaymentPlanBase
    {
        public virtual T PaymentPlan { get; set; }
    }
}
