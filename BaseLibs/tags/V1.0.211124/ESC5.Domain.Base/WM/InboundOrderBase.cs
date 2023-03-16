using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using eBPM.Role;
using System.Collections.Generic;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public abstract class InboundOrderBase : BaseDomainObject
    {

        #region "persistent properties"
        public virtual string InboundOrderNo { get; set; }
        public virtual DateTime CreatedTime { get; set; }

        #endregion

        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }
    [MappingIgnore]
    public abstract partial class InboundOrderBase<ItemT> : InboundOrderBase where ItemT : InboundOrderItemBase
    {
        public virtual IList<ItemT> Items { get; set; }
        public abstract void AddItem(ItemT item);
    }
}

