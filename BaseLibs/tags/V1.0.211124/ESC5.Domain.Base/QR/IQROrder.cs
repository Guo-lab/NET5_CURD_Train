using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eBPM.Role;

namespace ESC5.Domain.Base.QR
{
    public interface IQROrder
    {
        int Id { get;  }
        string QRNo { get; set; }
        int Status { get; set; }
        DateTime? StartedTime { get; set; }
        DateTime? ExpiredTime { get; set; }
        bool CanbeSubmittedby(IUser user);               
        bool CanbeCancelledby(IUser user);
        bool CanbePublishedby(IUser user);
        bool CanbeClosedby(IUser buyer);
        void Quote(IEnumerable<QuotationDTO> quotationList, IUser buyer);
        void Extend(DateTime extendTo);
        void Open();
    }

}
