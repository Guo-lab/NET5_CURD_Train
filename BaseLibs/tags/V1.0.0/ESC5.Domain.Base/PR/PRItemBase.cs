using eBPM.DomainModel.WorkFlowObject;
using eBPM.Role;
using ESC5.ValueObject;
using ProjectBase.Domain;
using System;
namespace ESC5.Domain.Base.PR
{

    [MappingIgnore]
    public  abstract partial class PRItemBase<ProcessT,ProcessorT> : BaseWorkFlowObject<ProcessT,ProcessorT> where ProcessT : BaseProcess
                                                                                                where ProcessorT : BaseCurrentProcessor
    {
        
        #region "persistent properties"

		public virtual string PRNo { get; set; }
		public virtual int ItemNo { get; set; }
		public virtual DateTime RequestedTime { get; set; }
        public virtual bool CapitalExpense { get; set; }
        public virtual string Requestor { get; set; }
		public virtual string? RequestorExtNo { get; set; }
        public virtual string? PurchasingGroup { get; set; }
        public virtual string? PartNo { get; set; }
		public virtual string Description { get; set; }
		public virtual UnitVO Unit { get; set; }
		public virtual decimal Quantity { get; set; }
		public virtual decimal OpenQuantity { get; set; }
		public virtual DateTime? RequiredDate { get; set; }
		public virtual IUser? Buyer { get; protected set; }
		public virtual IUser? BuyerOwner { get; protected set; }
        
        #endregion
    }
}