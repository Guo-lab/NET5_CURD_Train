using eBPM.Role;
using ESC5.Domain.Base.VD;
using System;
namespace ESC5.Domain.Base.QR
{
    public interface IQRInquiry
    {
        void Reply(string response, bool toAllVendor,IUser repliedby);
        void Ask(IQROrder qr, IVendor vendor, string inquiryText, DateTime inquiryTime);
    }
}
