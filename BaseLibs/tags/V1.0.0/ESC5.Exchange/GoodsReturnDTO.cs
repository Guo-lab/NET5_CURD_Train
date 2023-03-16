using ESC5.ValueObject;
using System;
using System.Collections.Generic;
namespace ESC5.Exchange
{
    public class GoodsReturnDTO
    {
        public int Id { get; set; }
        public string RequestNo { get; set; }
        public int VendorId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedbyName { get; set; }
        public IList<GoodsReturnItemDTO> Items { get; set; }
    }

    public class GoodsReturnItemDTO
    {
        public int Id { get; set; }
        public int GoodsReturnId { get; set; }
        public string PONo { get; set; }
        public string POItemNo { get; set; }
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
        public decimal Quantity { get; set; }
        public string Remark { get; set; }

    }
}
