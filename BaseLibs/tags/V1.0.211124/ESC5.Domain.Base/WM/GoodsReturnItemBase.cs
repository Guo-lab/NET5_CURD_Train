using System;
using ESC5.ValueObject;
using ProjectBase.Domain;
using ProjectBase.Utils;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public class GoodsReturnItemBase : BaseDomainObject
    {
        #region "persistent properties"
        public virtual int ItemNo { get; set; }
        public virtual string? LotNo { get; set; }
		public virtual UnitVO Unit { get; set; }
		public virtual Decimal Quantity { get; set; }
		public virtual string? Remark { get; set; }

        #endregion

        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

}

