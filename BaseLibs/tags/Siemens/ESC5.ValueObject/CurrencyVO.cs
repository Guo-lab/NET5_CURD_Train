using System;
using Newtonsoft.Json;
namespace ESC5.ValueObject
{
    public class CurrencyVO : SharpArch.Domain.DomainModel.ValueObject
    {
        [JsonProperty]
        public virtual string CurrencyCode { get; protected set; }
        [JsonProperty]
        public virtual string CurrencyDescription { get; protected set; }
        [JsonProperty]
        public virtual decimal ExchangeRate { get; protected set; }
        //阻止缺省实例化
        private CurrencyVO()
        {
            
        }
        public CurrencyVO(string code, string description, decimal rate)
        {
            this.CurrencyCode = code;
            this.CurrencyDescription = description;
            this.ExchangeRate = rate;
        }
    }
}
