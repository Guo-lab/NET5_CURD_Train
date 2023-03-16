using ESC5.ValueObject;
using System;

namespace ESC5.Domain.Base.QR
{
    public class QuotationDTO
    {
        public int Id { get; set; }
        public bool Participate { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public CurrencyVO? Currency { get; set; }
        public TaxRateVO? TaxRate { get; set; }
        public PaymentTermVO? PaymentTerm { get; set; }
        public TradingTermVO? TradingTerm { get; set; }
        public int? LeadTime { get; set; }
        public decimal? QuotedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? QuotedPriceVAT { get; set; }
        public decimal? FinalPriceVAT { get; set; }
        public DateTime QuoteTime { get; set; }


    }
}
