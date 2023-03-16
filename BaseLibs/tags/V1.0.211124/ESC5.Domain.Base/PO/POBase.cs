using eBPM.DomainModel.WorkFlowObject;
using eBPM.Role;
using ESC5.Domain.Base.VD;
using ESC5.ValueObject;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public abstract class POBase<ProcessT, ProcessorT> : BaseWorkFlowObject<ProcessT, ProcessorT> where ProcessT : BaseProcess, new()
                                                                                                where ProcessorT : BaseCurrentProcessor
    {
        public virtual int? Version { get; set; }
        public virtual string PONo { get; set; }
        public virtual DateTime RequestedTime { get; set; }
        public virtual bool CapitalExpense { get; set; }

        public virtual IVendor Vendor { get; set; }
        public virtual string VendorName { get; set; }
        public virtual DateTime? ApprovedTime { get; set; }
        public virtual CurrencyVO Currency { get; set; }
        public virtual PaymentTermVO PaymentTerm { get; set; }
        public virtual TradingTermVO TradingTerm { get; set; }
        public virtual PaymentModeVO PaymentMode { get; set; }
        public virtual IUser Buyer { get; set; }
        public virtual DateTime? SyncTime { get; set; }
        public virtual DateTime? ReceivedTime { get; set; }

        public virtual int ReceivingStatus { get; set; }
        public virtual bool NeedSettlement { get; set; }
        public virtual int SettlementStatus { get; set; }

        public virtual DateTime? EffectiveFrom { get; set; } //BO有效期开始
        public virtual DateTime? EffectiveTo { get; set; } //BO有效期截止
        public virtual Guid SourceKey { get; set; }

        public virtual bool IsActive { get; set; } //最新版本为true
    }

    [MappingIgnore]
    public abstract partial class POBase<ProcessT, ProcessorT, ItemT> : POBase<ProcessT,ProcessorT> where ProcessT : BaseProcess, new()
                                                                                                where ProcessorT : BaseCurrentProcessor
                                                                                                where ItemT : POItemBase
                                                                                              
    {

        public POBase()
        {
            this.SourceKey = Guid.NewGuid();
        }
        public virtual IList<ItemT> Items { get; set; }
    }

}

