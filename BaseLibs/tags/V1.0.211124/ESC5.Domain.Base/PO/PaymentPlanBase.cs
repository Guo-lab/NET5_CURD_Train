using System;
using System.Collections.Generic;
using System.Linq;
using ESC5.ValueObject;
using ProjectBase.Domain;

namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public class PaymentPlanBase:BaseDomainObject
    {
        public virtual decimal AmountVAT { get; set; }
        public virtual string Comments { get; set; }
        public virtual DateTime? PlannedPaymentDate { get; set; }
        public virtual DateTime? ActualPaidDate { get; set; }
        public virtual PaymentModeVO PaymentMode { get; set; }
        public virtual string PaymentCondition { get; set; }
        public virtual bool IsDiff { get; set; }

        public virtual bool PaymentRequestIssued { get; set; }
        public virtual DateTime? SyncTime { get; set; }
        public virtual Guid SourceKey { get; set; }

        public PaymentPlanBase()
        {
            this.SourceKey = Guid.NewGuid();
        }
    }
}
