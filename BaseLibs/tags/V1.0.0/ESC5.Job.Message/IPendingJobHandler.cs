using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Job.Message
{
    public interface IPendingJobHandler
    {
        void UpdatePendingJob(string OrderID);
    }
}
