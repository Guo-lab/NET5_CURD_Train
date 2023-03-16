using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESC5.Job.Message;

namespace ESC5.PendingJob.Service.Handler
{
    public class PendingJobHandlerFactory
    {
        public static IPendingJobHandler CreateHandler(string OrderType)
        {
            return (IPendingJobHandler)Activator.CreateInstance(Type.GetType("ESC5.PendingJob.Service.Handler." + OrderType + "JobHandler"));
        }
    }
}
