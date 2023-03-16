using System;
using System.Collections.Generic;

namespace ProjectBase.BusinessDelegate
{
    public interface IApplicationStorage
    {
        bool ContainsKey(string key);
        object Get(string key);
        void Set(string key, object obj);
        string GetRealPath(string relativePath);
        public string GetAppSetting(string key);
    }
}