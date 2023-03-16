using ESC5.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Exchange
{
    public class QRBaseDTO
    {
        public int Id { get; set; }
        public string QRNo { get; set; }
        public DateTime QRTime { get; set; }
        public bool Urgent { get; set; }
        public string BuyerName { get; set; }
        public int QuotationMode { get; set; }
        public DateTime ExpiredTime { get; set; }
        public bool Closed { get; set; }
    }

    public class QRItemBaseDTO
    {
        public int Id { get; set; }
        public int ItemNo { get; set; }
        public string PartNo { get; set; }
        public string Description { get; set; }
        public string DrawingNo { get; set; }
        public string RevisionNo { get; set; }
        public string Brand { get; set; }
        public virtual string ChineseDescription { get; set; }
        public virtual UnitVO Unit { get; set; }
        public decimal Quantity { get; set; }
        public virtual CurrencyVO PriceLimitCurrency { get; set; }
        public virtual decimal? PriceLowerLimit { get; set; }
        public virtual decimal? LowerToleranceAmount { get; set; }
        public virtual decimal? LowerTolerancePercentage { get; set; }
        public virtual decimal? PriceUpperLimit { get; set; }
        public virtual decimal? UpperToleranceAmount { get; set; }
        public virtual decimal? UpperTolerancePercentage { get; set; }
    }
}
