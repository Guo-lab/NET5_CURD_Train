using Newtonsoft.Json;

namespace ESC5.ValueObject
{
   public class TradingTermVO : SharpArch.Domain.DomainModel.ValueObject
    {
        [JsonProperty]
        public virtual string TradingTermCode { get; protected set; }
        [JsonProperty]
        public virtual string TradingTermDescription { get; protected set; }
        
        private TradingTermVO()
        {
           
        }
        public TradingTermVO(string code, string description)
        {
            TradingTermCode = code;
            TradingTermDescription = description;
        }
    }
}
