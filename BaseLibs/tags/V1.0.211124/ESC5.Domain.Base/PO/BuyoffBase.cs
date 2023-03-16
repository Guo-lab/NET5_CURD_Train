using eBPM.Role;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;

namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public abstract class BuyoffBase : BaseDomainObject
    {
        #region "persistent properties"

        public virtual string BuyoffNo { get; set; }
        public virtual string DeliveryNo { get; set; }
        public virtual DateTime BuyoffDate { get; set; }
        public virtual DateTime CreatedTime { get; set; }
        public virtual IUser Createdby { get; set; }
        public virtual DateTime? SyncTime { get; set; }

        #endregion

        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

    [MappingIgnore]
    public abstract partial class BuyoffBase<ItemT>: BuyoffBase where  ItemT: BuyoffItemBase
    {
        public virtual IList<ItemT> Items { get; set; }
        public abstract void AddItem(ItemT item);
    }

}

