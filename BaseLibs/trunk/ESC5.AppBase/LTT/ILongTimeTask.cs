using System;
using System.Collections.Generic;

namespace ESC5.AppBase.LTT
{
    public interface ILongTimeTask
    {
        int Operation { get; }
        void Execute(string traceKey);
        string GetTraceKey(LongTimeTaskTrace trace);
        object GetLockObj(object? lockSource);
    }
 }

