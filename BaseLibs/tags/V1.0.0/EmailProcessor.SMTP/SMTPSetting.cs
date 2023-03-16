using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailProcessor
{
    public class SMTPSetting
    {
        public string CustomerCode { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPUID { get; set; }
        public string SMTPPWD { get; set; }

        public string Sender { get; set; }
    }
}
