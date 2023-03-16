using ESC5.ValueObject;
using ProjectBase.Domain;

namespace ESC5.Domain.Base.PO
{
    [MappingIgnore]
    public abstract class BuyoffItemBase : BaseDomainObject
    {
        #region "persistent properties"        
		public virtual string LotNo { get; set; }
        public virtual UnitVO Unit { get; set; }
		public virtual decimal BuyoffQuantity { get; set; }
        public virtual decimal? SettlementPrice { get; set; }
        public virtual string Remark { get; set; }
        #endregion


        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

}

