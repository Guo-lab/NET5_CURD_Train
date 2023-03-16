using eBPM.Role;
using ProjectBase.Domain;
using System;
using ESC5.Domain.Base.VD;
namespace ESC5.Domain.Base.QR
{
    [MappingIgnore]
    public abstract class QRInquiryBase:VersionedDomainObjectWithAssignedIntId,IQRInquiry
    {
        public virtual void Reply(string response, bool toAllVendor,IUser repliedby)
        {
            if (! this.ResponsedTime.HasValue)
            {
                this.Response = response;
                this.ToAllVendor = toAllVendor;
                this.Responsedby = repliedby;
                this.ResponsedTime = DateTime.Now;
                this.SyncTime = null;
            }
        }
        public virtual void Ask(IQROrder qr, IVendor vendor, string inquiryText, DateTime inquiryTime)
        {
            this.QR = qr;
            this.Vendor = vendor;
            this.Inquiry = inquiryText;
            this.InquiryTime = inquiryTime;
        }

        public virtual IQROrder QR { get; set; }
        public virtual IVendor Vendor { get; set; }

        public virtual DateTime InquiryTime { get; protected set; }
        public virtual string Inquiry { get; protected set; }
        public virtual string Response { get; protected set; }
        public virtual bool ToAllVendor { get; protected set; }
        public virtual IUser Responsedby { get; protected set; }
        public virtual DateTime? ResponsedTime { get; protected set; }
        public virtual DateTime? SyncTime { get; set; }
        public virtual DateTime? ReceivedTime { get; set; }
    }
}
