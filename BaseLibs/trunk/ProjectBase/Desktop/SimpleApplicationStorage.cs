using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using ProjectBase.BusinessDelegate;
namespace ProjectBase.Desktop
{


    public class SimpleApplicationStorage : IApplicationStorage
    {
        public IConfiguration Configuration { get; set; }

        private readonly Dictionary<string, object> storage = new Dictionary<string, object>();

        public bool ContainsKey(string key)
        {
            return storage.ContainsKey(key);
        }

        public object Get(string key)
        {
            object obj;

            if (!this.storage.TryGetValue(key, out obj))
            {
                return null;
            }

            return obj;
        }


        public void Set(string key, object obj)
        {
            this.storage[key] = obj;
        }
        public string GetRealPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + relativePath;
        }
        public string GetAppSetting(string key)
        {
            return Configuration["AppSetting" + ":" + key];
        }
    }
}