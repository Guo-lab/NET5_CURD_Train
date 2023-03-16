using System;

namespace E2biz.Utility.General
{
    public interface IDateService
    {
        DateTime Now
        {
            get;
        }
        DateTime Today { get; }
    }
}