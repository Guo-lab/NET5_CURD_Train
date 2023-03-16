using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using eBPM.Role;
using System.Collections.Generic;
using ESC5.ValueObject;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public abstract class TransferOrderItemBase : BaseDomainObject
    {
        #region "persistent properties"
        public virtual string? LotNo { get; set; }
        public virtual decimal Quantity { get; set; }

        ///物料主单位（需换算）
        public virtual UnitVO Unit { get; set; }

        #endregion

        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }
}

