using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Exchange
{
    public class VendorE2bizDTO
    {
        public int Id { get; set; }
        public string E2bizCode { get; set; }
    }
    public class VendorDTO
    {
        public int Id { get; set; }
        public int VendorStatus { get; set; }
        public string Code { get; set; }
        public string E2bizCode { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string SWIFTCode { get; set; }
        public string EstablishTime { get; set; }
        public string RegisteredCapital { get; set; }
        public string RegistrationCode { get; set; }
        public string LegalPerson { get; set; }
        public string AccountNo { get; set; }
        public string IssuingBank { get; set; }
        public string BusinessScope { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
        public string BuyerName { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentTermId { get; set; }
        public int TradingTermId { get; set; }
        public int TaxRateId { get; set; }
        public int PaymentModeId { get; set; }
        
        public DateTime LastModifiedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ApprovedTime { get; set; }

  
        public IList<VendorContactDTO> Contacts { get; set; }

    }

    public class VendorContactDTO
    {
        public int Id { get; set; }
        public string ContactName { get; set; }

        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        
    }

}
