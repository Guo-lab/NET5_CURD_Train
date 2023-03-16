using ESC5.Domain.Base.VD;
using ESC5.ValueObject;
using System.Collections.Generic;

namespace ESC5.Domain.Base.PR
{
    public class POGenerationDTO
    {
        public int PRItemId { get; set; }
        public IList<SelectedQuotationDTO> Quotations { get; set; }

    }
    public class SelectedQuotationDTO
    {
        public IVendor Vendor { get; set; }
        public string QuotationNo { get; set; }
        public string? VendorPN { get; set; }
        public CurrencyVO Currency { get; set; }
        public PaymentTermVO PaymentTerm { get; set; }
        public TradingTermVO TradingTerm { get; set; }
        public PaymentModeVO PaymentMode { get; set; }
        public TaxRateVO TaxRate { get; set; }

        public decimal Quantity { get; set; }
        public string UnitCode { get; set; }

        public decimal UnitPrice { get; set; }
        public int LeadTime { get; set; }
    }
}
