using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Email.Service
{
    public static class AppSetting
    {
        public static string EmailChannel { get; set; }
        public static string CustomerCode { get; set; }
        public static int EmailRemotingPort { get; set; }
    }
}
