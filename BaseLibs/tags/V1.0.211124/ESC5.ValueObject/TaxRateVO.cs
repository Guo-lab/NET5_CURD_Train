using Newtonsoft.Json;
using ProjectBase.Domain;
namespace ESC5.ValueObject
{
    public class TaxRateVO : SharpArch.Domain.DomainModel.ValueObject
    {
        [JsonProperty]
        public  string TaxRateCode { get; protected set; }
        [JsonProperty]
        public  string TaxRateDescription { get; protected set; }
        [JsonProperty]
        public  decimal TaxRateValue { get; protected set; }

        [MappingIgnore]
        [JsonIgnore]
        public string PercentageValue
        {
            get
            {
                return this.TaxRateValue.ToString("P", new System.Globalization.NumberFormatInfo
                {
                    PercentDecimalDigits = 0,//小数点保留几位数. 
                    PercentPositivePattern = 1//百分号出现在何处. 
                });
            }
        }

        private TaxRateVO()
        {

        }
        public TaxRateVO(string code, string description, decimal rate)
        {
            TaxRateCode = code;
            TaxRateDescription = description;
            TaxRateValue = rate;
        }
        
    }
}
