using ESC5.ValueObject;
using System;
using System.Collections.Generic;
namespace ESC5.Exchange
{
    public class ReceivingDTO 
    {
        public int Id { get; set; }
        public string ReceivingNo { get; set; }
        public string DeliveryNo { get; set; }
        public int VendorId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public IList<ReceivingItemDTO> Items { get; set; }
    }

    public class ReceivingItemDTO
    {
        public int Id { get; set; }
        public int ReceivingId { get; set; }
        public virtual string PONo { get; set; }
        public virtual int POItemNo { get; set; }
        public string PartNo { get; set; }        
        public string Description { get; set; }
        public string ChineseDescription { get; set; }
        public string Spec { get; set; }
        public string VendorPN { get; set; }
        public string DrawingNo { get; set; }
        public string RevisionNo { get; set; }
        public string Brand { get; set; }
        public string LotNo { get; set; }
        public string UnitCode { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public string Remark { get; set; }
        public IList<ReceivingItemAttrDTO> Attributes { get; set; }

    }

    public class ReceivingItemAttrDTO {
        public int Id { get; set; }
        public int ReceivingItemId { get; set; }
        public virtual string PartAttrName { get; set; }
        public virtual string PartAttrValue { get; set; }
    }
}
