using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2biz.Utility.General
{
    public class SystemDateService : IDateService
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
        public DateTime Today
        {
            get
            {
                return DateTime.Today;
            }
        }
    }
}
