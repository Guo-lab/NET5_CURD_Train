using Newtonsoft.Json;

namespace ESC5.ValueObject
{
   public class PaymentTermVO : SharpArch.Domain.DomainModel.ValueObject
    {
        [JsonProperty]
        public virtual string PaymentTermCode { get; protected set; }
        [JsonProperty]
        public virtual string PaymentTermDescription { get; protected set; }
        [JsonProperty]
        public virtual int PaymentDays { get; protected set; }

        private PaymentTermVO()
        {
        }
        public PaymentTermVO(string code, string description,int paymentDays)
        {
            PaymentTermCode = code;
            PaymentTermDescription = description;
            PaymentDays = paymentDays;
        }
    }
}
