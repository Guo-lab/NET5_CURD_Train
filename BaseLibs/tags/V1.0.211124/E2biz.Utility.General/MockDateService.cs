using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace E2biz.Utility.General
{
    public class MockDateService : IDateService
    {
        private const string CACHEKEY = "MockDate";
        public IMemoryCache Cache { get; set; }

        private PhysicalFileProvider _provider;

        private string _mockFile;
        public MockDateService(string mockFile)
        {
            _mockFile = mockFile;
        }
        private DateTime MockDate {
            get
            {
                string path = Path.GetDirectoryName (_mockFile);
                string configFile = Path.GetFileName (_mockFile);
                if (_provider == null)
                {
                    _provider = new PhysicalFileProvider(path);
                }
                return Cache.GetOrCreate(CACHEKEY, (ICacheEntry entry) =>
                {
                    DateTime now;
                    try
                    {
                        now = Convert.ToDateTime(File.ReadAllText(_mockFile));
                    }
                    catch
                    {
                        now = DateTime.Now;
                    }
                    PhysicalFileProvider provider = new PhysicalFileProvider(path); ;
                    entry.AddExpirationToken(_provider.Watch(configFile));
                    return now;
                });
            }
            
        }
       
        public DateTime Now
        {
            get
            {
                return this.MockDate;
            }
        }

        public DateTime Today
        {
            get
            {
                return this.MockDate.Date;
            }
        }
    }
}
