using System;
using System.Collections.Generic;

namespace ESC5.AppBase.LTT
{
    public class LongTimeTaskTrace
    {
        public string TraceKey { get; set; }
        public int Operation { get; set; }

        public object LockObj { get; set; }

        public int User { get; set; }

        public DateTime StartTime { get; set; }

        public int ProgressNum { get; set; }
        public string ProgressInfo { get; set; }

        public IList<int> Subscribers { get; set; } = new List<int>();

    }

}

