using ESC5.ValueObject;
using System;
using System.Collections.Generic;
namespace ESC5.Exchange
{
    public class QRDTO:QRBaseDTO
    {
        public bool TechEvaluationRequired { get; set; }
        public string RemarkToVendor { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string BuyerEmail { get; set; }
        public IList<QRItemDTO> Items { get; set; }
        public IList<QRAttachmentDTO> Attachments { get; set; }
        public DateTime? StartedTime { get; set; }
    }

    public class QRItemDTO:QRItemBaseDTO
    {
       
        public string Requestor { get; set; }
        public string RequestorExtNo { get; set; }
        public DrawingDTO Drawing { get; set; }
        public string Remark { get; set; }
        public DateTime? RequiredDate { get; set; }
        public IList<QRAttachmentDTO> Attachments { get; set; }
        public IList<QuotationDTO> Quotations { get; set; }
        public IList<QRAttrDTO> Attributes { get; set; }
    }
    public class QRAttrDTO {
        public int Id { get; set; }
        public  string PartAttrName { get; set; }
        public  string PartAttrValue { get; set; }

    }

    public class QuotationDTO
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string VendorEmail { get; set; }
        public decimal? Weight { get; set; }
        public string VendorPN { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public CurrencyVO Currency { get; set; }
        public PaymentTermVO PaymentTerm { get; set; }
        public TradingTermVO TradingTerm { get; set; }
        public TaxRateVO TaxRate { get; set; }
        public PaymentModeVO PaymentMode { get; set; }
        public int? LeadTime { get; set; }
        public decimal? QuotedPrice { get; set; }
        public string Remark { get; set; }
        public bool Participate { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime ExpiredTime { get; set; }
        public DateTime? QuoteTime { get; set; }
        public string QuoteIPAddress { get; set; }

        public int Round { get; set; }
    }

    //从外网传回的QuotationDTO
    public class VendorQuotationDTO 
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public CurrencyVO? Currency { get; set; }
        public PaymentTermVO? PaymentTerm { get; set; }
        public TradingTermVO? TradingTerm { get; set; }
        public TaxRateVO? TaxRate { get; set; }
        public PaymentModeVO? PaymentMode { get; set; }
        public int? LeadTime { get; set; }
        public decimal? QuotedPrice { get; set; }
        public decimal? QuotedPriceVAT { get; set; }
        public string? Remark { get; set; }
        public bool Participate { get; set; }
        public DateTime QuoteTime { get; set; }
        public string QuoteIPAddress { get; set; }
        public IList<QuotationAttachmentDTO> QuotationAttachments { get; set; }
    }
    
    public class QRAttachmentDTO : SyncLib.Exchange.Attachment
    {
        public QRAttachmentDTO() : base("Attachment\\QR", "Attachment\\QR")
        {

        }
        public int Id { get; set; }
        public int QRId { get; set; }
        public int QRItemId { get; set; }
    }

    public class QuotationAttachmentDTO : SyncLib.Exchange.Attachment
    {
        public QuotationAttachmentDTO() : base("Attachment\\Quotation", "Attachment\\Quotation")
        {

        }
        public DateTime UploadedTime { get; set; }
    }
}
