using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Job.Message
{
    [Serializable]
    public class JobOrder
    {
        public string OrderType { get; set; }
        public string OrderID { get; set; }
    }
}
