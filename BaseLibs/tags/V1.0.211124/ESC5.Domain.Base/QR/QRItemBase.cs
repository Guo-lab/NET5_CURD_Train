using ProjectBase.Domain;
using ESC5.ValueObject;
using System.Collections.Generic;

namespace ESC5.Domain.Base.QR
{
    //QRItem基础类，每个具体的项目应该继承此类加入特定的属性
    [MappingIgnore]
    public class QRItemBase : BaseDomainObject,IQRItem
    {

        #region "persistent properties"
        public virtual int ItemNo { get; set; }
		public virtual string? PartNo { get; set; }
		public virtual string Description { get; set; }
		public virtual UnitVO Unit { get; set; }
		public virtual decimal Quantity { get; set; }
        #endregion

    }

}

