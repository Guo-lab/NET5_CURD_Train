using ESC5.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Exchange
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public int VendorId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public decimal AmountVAT { get; set; }
        public  CurrencyVO Currency { get; set; }
        public  bool IsActive { get; set; }
        public  bool InUse { get; set; }
        public  DateTime? InvalidDate { get; set; }
        public IList<InvoiceItemDTO> Items { get; set; }
    }

    public class InvoiceItemDTO
    {
        public int Id { get; set; }
        public int? PaymentPlanId { get; set; }
        public string PONo { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public decimal AmountVAT { get; set; }
    }
}
