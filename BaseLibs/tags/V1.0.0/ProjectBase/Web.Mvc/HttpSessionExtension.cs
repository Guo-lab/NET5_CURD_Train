using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ProjectBase.Web.Mvc
{
    public static class HttpSessionExtension
    {
        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value==null || string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value);
        }
        public static void Set<T>(this ISession session, string key,T obj) 
        {
            string value = JsonConvert.SerializeObject(obj);
            session.SetString(key,value);
        }
        public static bool ContainsKey(this ISession session, string key)
        {
            return session.Keys.Contains(key);
        }
    }
}
