using ESC5.ValueObject;
using System;
using System.Collections.Generic;
namespace ESC5.Exchange
{
    public class ASNDTO 
    {
        public Guid Id { get; set; }
        public string ASNNo { get; set; }
        public int VendorId { get; set; }
        public string DeliveryNo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Status { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public DateTime? ArrivedTime { get; set; }
        public string ReceivedbyName { get; set; }
        public IList<ASNItemDTO> Items { get; set; }
    }

    public class ASNItemDTO
    {
        public Guid Id { get; set; }
        public int ASNItemNo { get; set; }
        public int POItemId { get; set; }
        public Decimal? ReceivedQuantity { get; set; }        
        public string LotNo { get; set; }
        public Decimal DeliveredQuantity { get; set; }
        public UnitVO Unit { get; set; }
        public string Remark { get; set; }
        public ASNSettlementExtDTO ASNSettlementExt { get; set; }
    }
    public class ASNSettlementExtDTO
    {
        public Decimal? SettlementPrice { get; set; }
    }
}
