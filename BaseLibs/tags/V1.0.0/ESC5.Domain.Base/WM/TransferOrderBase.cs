using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using eBPM.Role;
using System.Collections.Generic;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public abstract class TransferOrderBase : BaseDomainObject
    {

        #region "persistent properties"
		public virtual string TransferOrderNo { get; set; }
		public virtual DateTime CreatedTime { get; set; }
		public virtual IUser Createdby { get; set; }

        #endregion


        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

    [MappingIgnore]
    public abstract partial class TransferOrderBase<ItemT> : TransferOrderBase where ItemT : TransferOrderItemBase
    {
        public virtual IList<ItemT> Items { get; set; }
        public abstract void AddItem(ItemT item);
    }
}

