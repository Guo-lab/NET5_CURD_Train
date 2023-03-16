using ESC5.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Exchange
{
    public class BuyoffDTO
    {
        public int Id { get; set; }
        public string BuyoffNo { get; set; }
        public int VendorId { get; set; }
        public string DeliveryNo { get; set; }
        public DateTime BuyoffDate { get; set; }
        public IList<BuyoffItemDTO> Items { get; set; }
    }

    public class BuyoffItemDTO
    {
        public int Id { get; set; }
        public int BuyoffId { get; set; }
        public int ItemNo { get; set; }
        public string PONo { get; set; }
        public int POItemNo { get; set; }
        public int PartId { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public string ChineseDescription { get; set; }
        public string Spec { get; set; }
        public string VendorPN { get; set; }
        public string DrawingNo { get; set; }
        public string RevisionNo { get; set; }
        public string Brand { get; set; }
        public UnitVO Unit { get; set; }
        public string PrimaryUnitCode { get; set; }

        public decimal ConversionRate { get; set; }
        public decimal? SettlementPrice { get; set; }
        public decimal BuyoffQuantity { get; set; }
        public string Remark { get; set; }
        public string LotNo { get; set; }
        public IList<BuyoffItemAttrDTO> Attributes { get; set; }
    }
    public class BuyoffItemAttrDTO
    {
        public int Id { get; set; }
        public int BuyoffItemId { get; set; }
        public virtual string PartAttrName { get; set; }
        public virtual string PartAttrValue { get; set; }
    }

}
