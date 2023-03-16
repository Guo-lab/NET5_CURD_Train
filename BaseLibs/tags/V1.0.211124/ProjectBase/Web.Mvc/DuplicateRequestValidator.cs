using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectBase.Web.Mvc
{
    public class DuplicateRequestValidator
    {
        public static void Verify(int delayRequest, ActionExecutingContext filterContext)
        {
            /****TODO:
            var request = filterContext.HttpContext.Request;
            Cache cache = filterContext.HttpContext.Cache;

            // 以用户的IP+浏览器Agent+请求的URL作为Key
            string key = (request.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? request.UserHostAddress)
                                           + request.UserAgent
                                           + request.RawUrl + request.QueryString
                                           + filterContext.ActionDescriptor.ActionName() + JsonConvert.SerializeObject(filterContext.ActionParameters());

            // key加密后作为Cache的Key
            string hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(key)).Select(s => s.ToString("x2")));

            if (cache[hashValue] != null)
            {
                // Adds the Error Message to the Model and Redirect
                throw new DuplicatedRequestException();
            }
            else
            {
                try
                {
                    cache.Add(hashValue, "", null, DateTime.Now.AddSeconds(delayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
                catch
                {
                    throw new DuplicatedRequestException();
                }
            }*/
        }
    }
}
