using Newtonsoft.Json;
namespace ESC5.ValueObject
{

    public class UnitVO : SharpArch.Domain.DomainModel.ValueObject
    {
        [JsonProperty]
        public virtual string UnitCode { get; protected set; }
        [JsonProperty]
        public virtual string? PrimaryUnitCode { get; protected set; }
        [JsonProperty]
        public virtual decimal? ConversionRate { get; protected set; }

        public UnitVO()
        {

        }
        public UnitVO(string unitCode, string? primaryUnitCode, decimal? conversionRate)
        {
            this.UnitCode = unitCode;
            this.PrimaryUnitCode = primaryUnitCode;
            this.ConversionRate = conversionRate;
        }
        public static UnitVO NoConversionUnit(string unitCode)
        {
            return new UnitVO(unitCode, null, null);
        }
        /// <summary>
        /// 转为主单位数量
        /// </summary>
        /// <param name="qty"></param>
        /// <returns></returns>
        public decimal ConvertToPrimaryUnitQty(decimal qty)
        {
            return qty * this.ConversionRate!.Value;
        }
        /// <summary>
        /// 转为目标单位数量
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="destUnit"></param>
        /// <returns></returns>
        public decimal ConvertToDestUnitQty(decimal qty, UnitVO destUnit)
        {
            if (destUnit.ConversionRate.HasValue)
            {
                return qty / this.ConversionRate!.Value * destUnit.ConversionRate.Value;
            }
            return qty;
        }
        /// <summary>
        /// 转为主单位价格
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public decimal ConvertToPrimaryUnitPrice(decimal price)
        {
            return price / this.ConversionRate!.Value;
        }
        /// <summary>
        /// 转为目标单位价格
        /// </summary>
        /// <param name="price"></param>
        /// <param name="destUnit"></param>
        /// <returns></returns>
        public decimal ConvertToDestUnitPrice(decimal price, UnitVO destUnit)
        {
            return price / this.ConversionRate!.Value * destUnit.ConversionRate!.Value;
        }
    }
}
