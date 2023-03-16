using System;
using System.Collections.Generic;
using eBPM.Role;
using ESC5.Domain.Base.VD;
using ProjectBase.Domain;
using ProjectBase.Utils;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public abstract class ReceivingBase : BaseDomainObject
    {
        #region "persistent properties"

        public virtual string ReceivingNo { get; set; }
        public virtual IVendor Vendor { get; set; }
        public virtual string VendorName { get; set; }
        public virtual string? DeliveryNo { get; set; }
        public virtual DateTime? DeliveryDate { get; set; }
        public virtual DateTime ReceivedDate { get; set; }
        public virtual DateTime CreatedTime { get; set; }
        public virtual IUser Createdby { get; set; }
        public virtual DateTime? SyncTime { get; set; }
        public virtual DateTime? InspectionTime {get;set;}
     
        #endregion

        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

    [MappingIgnore]
    public abstract partial class ReceivingBase<ItemT>: ReceivingBase where  ItemT: ReceivingItemBase
    {
        public virtual IList<ItemT> Items { get; set; }
        public abstract void AddItem(ItemT item);
    }

}

