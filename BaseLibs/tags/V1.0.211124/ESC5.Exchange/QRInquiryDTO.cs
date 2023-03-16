using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESC5.ValueObject;
using System.Configuration;
namespace ESC5.Exchange
{
    /// <summary>
    /// 从外网下载到内容的Inquiry交换格式
    /// </summary>
    public class QRInquiryDTO
    {
        public int Id { get; set; }
        public int QRId { get; set; }
        public int VendorId { get; set; }
        public DateTime InquiryTime { get; set; }
        public string Inquiry { get; set; }
    }

   /// <summary>
   /// 从内网上传到外网的Inquiry交换格式
   /// </summary>
    public class QRInquiryResponseDTO
    {
        public int Id { get; set; }
        public int QRId { get; set; }
        public string Response { get; set; }
        public string ResponsedbyName { get; set; }
        public DateTime ResponsedTime { get; set; }
        public bool ToAllVendor { get; set; }
    }
 }
