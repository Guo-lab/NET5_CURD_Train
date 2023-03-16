
using Newtonsoft.Json;
namespace ESC5.ValueObject
{
   public class PaymentModeVO : SharpArch.Domain.DomainModel.ValueObject
    {
        [JsonProperty]
        public virtual string PaymentModeCode { get; protected set; }
        [JsonProperty]
        public virtual string PaymentModeDescription { get; protected set; }
        
        private PaymentModeVO()
        {
        }
        public PaymentModeVO(string code, string description)
        {
            PaymentModeCode = code;
            PaymentModeDescription = description;
        }
    }
}
