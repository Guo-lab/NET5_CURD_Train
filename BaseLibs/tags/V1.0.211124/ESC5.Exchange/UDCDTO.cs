using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Exchange
{
    public class UDCDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }

    public class CurrencyDTO : UDCDTO
    {
        public decimal ExchangeRate { get; set; }

    }

    public class TaxRateDTO : UDCDTO
    {
        public decimal Rate { get; set; }

    }
    public class TradingTermDTO : UDCDTO
    {

    }

    public class PaymentTermDTO : UDCDTO
    {
        public int PaymentDays { get; set; }
    }

    public class PaymentModeDTO : UDCDTO
    {

    }
    
}
