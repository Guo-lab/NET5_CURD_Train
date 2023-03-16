using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using eBPM.Role;
using System.Collections.Generic;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public abstract class GoodsReturnBase : BaseDomainObject
    {

        #region "persistent properties"
        public virtual string? VendorCode { get; set; }
        public virtual string? VendorName { get; set; }
        public virtual string GoodsReturnNo { get; set; }
		public virtual DateTime CreatedTime { get; set; }
		public virtual IUser Createdby { get; set; }
        #endregion


        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }
    [MappingIgnore]
    public abstract partial class GoodsReturnBase<ItemT> : GoodsReturnBase where ItemT : GoodsReturnItemBase
    {
        public virtual IList<ItemT> Items { get; set; }
        public abstract void AddItem(ItemT item);
    }
}

