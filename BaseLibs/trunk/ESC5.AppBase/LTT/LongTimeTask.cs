using System;
using System.Collections.Generic;

namespace ESC5.AppBase.LTT
{
    public abstract class LongTimeTask : ILongTimeTask
    {
        public abstract int Operation { get; }
        public abstract void Execute(string traceKey);
        public abstract object GetLockObj(object? lockSource);
        public virtual string GetTraceKey(LongTimeTaskTrace trace)
        {
            return trace.Operation.ToString() + "-" + trace.LockObj.GetHashCode().ToString();
        }

    }

}

