using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;

namespace E2biz.Utility.General
{
    public class MockDateService : IDateService
    {
        private const string CACHEKEY = "MockDate";
        public IMemoryCache Cache { get; set; }
        public IWebHostEnvironment Env { get; set; }

        private string _mockFile;
        public MockDateService(string mockFile)
        {
            _mockFile = mockFile;
        }
        private DateTime MockDate {
            get
            {
                return Cache.GetOrCreate<DateTime>(CACHEKEY, (ICacheEntry entry) =>
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
                    entry.AddExpirationToken(Env.ContentRootFileProvider.Watch(_mockFile));
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
