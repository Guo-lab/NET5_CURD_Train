using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ProjectBase.BusinessDelegate;
using System.Collections.Generic;
using System.IO;

namespace ProjectBase.Web.Mvc
{
    public class WebApplicationStorage : IApplicationStorage
    {
        IMemoryCache cache { get; set; }
        public IConfiguration Configuration { get; set; }
        public IWebHostEnvironment Env { get; set; }

        private static readonly IDictionary<string,object> _storage = new Dictionary<string, object>();

        public WebApplicationStorage()
        {
        }
        public bool ContainsKey(string key)
        {
            return _storage.ContainsKey(key);
        }
        public object Get(string key)
        {
            if (!ContainsKey(key)) return null;
            return _storage[key];
        }

        public void Set(string key, object obj)
        {
            _storage[key] = obj;
        }

        public string GetRealPath(string relativePath)
        {
            return Path.Combine(Env.ContentRootPath,relativePath); 
        }

        public string GetAppSetting(string key)
        {
            return Configuration["AppSetting" + ":"+key];
        }


    }
}