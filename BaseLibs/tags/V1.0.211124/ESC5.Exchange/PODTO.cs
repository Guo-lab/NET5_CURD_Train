using ESC5.ValueObject;
using System;
using System.Collections.Generic;
namespace ESC5.Exchange
{
    public class PODTO 
    {
        public int Id { get; set; }
        public string PONo { get; set; }
        public int Version { get; set; }
        public DateTime RequestedTime { get; set; }
        public bool CapitalExpense { get; set; }
        public string CARNo { get; set; }
        public int VendorId { get; set; }
        public DateTime ApprovedTime { get; set; }
        public CurrencyVO Currency { get; set; }
        public PaymentTermVO PaymentTerm { get; set; }
        public PaymentModeVO PaymentMode { get; set; }
        public TradingTermVO TradingTerm { get; set; }
        public string BuyerName { get; set; }
        public int POType { get; set; }       
        public int Status { get; set; }
        public int ReceivingStatus { get; set; }
        public bool NeedSettlement { get; set; }
        public string RemarkToVendor { get; set; }
        public DateTime LastModifiedTime { get; set; }

        public PrintTemplateDTO PrintTemplate { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public IList<POItemDTO> Items { get; set; }
        public IList<POAttachmentDTO> Attachments { get; set; }
    }

    public class POItemDTO
    {
        public int Id { get; set; }
        public int ItemNo { get; set; }
        public int BuyoffType { get; set; }
        public int PurchasingCategory { get; set; }
        public string Requestor { get; set; }
        public string RequestorExtNo { get; set; }

        public string PartNo { get; set; }
        public string Description { get; set; }
        public string ChineseDescription { get; set; }
        public string Spec { get; set; }
        public string VendorPN { get; set; }
        public string DrawingNo { get; set; }
        public string RevisionNo { get; set; }

        public DrawingDTO Drawing { get; set; }
        public string Brand { get; set; }
        
        public UnitVO Unit { get; set; }
        public int LeadTime { get; set; }
        public int Via { get; set; }
        public decimal UnitPrice { get; set; }
        //接收端会自动计算下面3个字段的值
        //public decimal UnitPriceVAT { get; set; }
        //public decimal Amount { get; set; }
        //public decimal AmountVAT { get; set; }
        public TaxRateVO TaxRate { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal OpenQuantity { get; set; }        
        public DateTime? RequestedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public Decimal OverloadingPercent { get; set; }
        public string QRNo { get; set; }
        public IList<POAttachmentDTO> Attachments { get; set; }
        public IList<POItemUnitDTO> Units { get; set; }
        public IList<POAttrDTO> Attributes { get; set; }
    }

    public class PrintTemplateDTO : SyncLib.Exchange.Attachment
    {
        public PrintTemplateDTO() : base("", "Attachment\\PO")
        {
        }
    }

    public class DrawingDTO : SyncLib.Exchange.Attachment
    {
        public DrawingDTO(string originalFileName, string savedFileName) : base("Attachment\\Drawing", "Attachment\\Drawing")
        {
            this.OriginalFileName = originalFileName;
            this.SavedFileName = savedFileName;
        }
    }

    public class POAttachmentDTO : SyncLib.Exchange.Attachment
    {
        public POAttachmentDTO() : base("Attachment\\PO", "Attachment\\PO")
        {

        }
        public int Id { get; set; }
        public int POId { get; set; }
        public int POItemId { get; set; }
    }

    public class POItemUnitDTO
    {
        public int Id { get; set; }
        public int POItemId { get; set; }
        public string Unit { get; set; }
        public bool IsPrimary { get; set; }
        public decimal Rate { get; set; }
    }

    public class POAttrDTO
    {
        public int Id { get; set; }
        public string PartAttrName { get; set; }
        public string PartAttrValue { get; set; }
    }
}
