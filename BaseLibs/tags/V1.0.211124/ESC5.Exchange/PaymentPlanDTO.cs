using System;
using ESC5.ValueObject;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Exchange
{
    public class PaymentPlanDTO
    {
        public int Id { get; set; }

        public int POId { get; set; }
        public int? POItemId { get; set; }
        public decimal AmountVAT { get; set; }
        public DateTime? PlannedPaymentDate { get; set; }
        public DateTime? ActualPaidDate { get; set; }

        public PaymentModeVO PaymentMode { get; set; }
        public string PaymentCondition { get; set; }
        public string Comments { get; set; }

    }

    
}
