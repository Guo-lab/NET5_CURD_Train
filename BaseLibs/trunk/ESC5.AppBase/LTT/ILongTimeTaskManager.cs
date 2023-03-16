using System;
using System.Collections.Generic;

namespace ESC5.AppBase.LTT
{
    public interface ILongTimeTaskManager
    {
        void Start(LongTimeTaskTrace trace, ILongTimeTask task, Action afterAction);
 //       void Finish(string traceKey);
        void UpdateProgress(string traceKey, int num, string info);
        bool IsLocked(string traceKey);
        IList<LongTimeTaskTrace> GetTraceList();
        IList<LongTimeTaskTrace> GetTraceList(int userId);
    }
 }

