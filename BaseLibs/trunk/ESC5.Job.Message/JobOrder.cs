using ESC5.OfflineService;
using System;

namespace ESC5.Job.Message
{
    [Serializable]
    public class JobOrder:TaskBase<string>
    {
        public string OrderType { get; set; }
    }
}
