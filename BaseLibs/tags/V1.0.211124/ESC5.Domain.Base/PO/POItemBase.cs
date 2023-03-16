using System;
using System.Collections.Generic;
using System.Linq;
using eBPM.Role;
using ProjectBase.Domain;
using ESC5.ValueObject;
using ESC5.Domain.Base.IM;


namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public abstract class POItemBase : VersionedDomainObject
    {
        public POItemBase()
        {
            this.SourceKey = Guid.NewGuid();
        }

        public virtual int ItemNo { get; set; }
        public virtual decimal Quantity { get; protected set; }
        public virtual decimal? ReceivedQuantity { get; protected set; }
        public virtual decimal? OpenQuantity { get; protected set; }
        public virtual decimal? BalanceQuantity { get; set; }
        public virtual decimal SettledQuantity { get; protected set; }
        public virtual UnitVO Unit { get; set; }
        public virtual decimal UnitPrice { get; protected set; }
        public virtual decimal UnitPriceVAT { get; protected set; }
        public virtual decimal Amount { get; protected set; }
        public virtual decimal AmountVAT { get; protected set; }
        public virtual TaxRateVO TaxRate { get; protected set; }

        public virtual IUser? Closedby { get; protected set; }
        public virtual DateTime? ClosedTime { get; protected set; }

        public virtual int? BuyoffType { get; set; }
        public virtual int? PurchasingCategory { get; set; }
        public virtual Guid? SourceKey { get; set; }

        public virtual DateTime? SyncTime { get; set; }

        #region "Methods"
        public virtual bool IsReceived()
        {
            return this.OpenQuantity <= 0 && !this.IsClosed();
        }
        public virtual bool IsClosed()
        {
            return this.ClosedTime.HasValue;
        }
        public abstract void RestoreBalanceQuantity();

        /// <summary>
        /// 调用此方法后需要调用POBase.CalcReceivingStatus方法
        /// </summary>
        /// <param name="closedBy"></param>
        public virtual void Close(IUser closedBy)
        {
            RestoreBalanceQuantity();
            this.Closedby = closedBy;
            this.ClosedTime = DateTime.Now;
            this.OpenQuantity = 0;
            this.SyncTime = null;
        }

        #endregion
    }
}
