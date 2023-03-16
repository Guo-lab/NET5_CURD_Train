using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjectBase.Web.Mvc.Validation
{
    /// <summary>
    /// [Range]、[Max]、[Min]三者只能使用其中一个.
    /// 限制值应与被验证值类型一致：日期型数据用字符串限制值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MinAttribute : RangeAttribute
    {
        public MinAttribute(int min) : this(typeof(int), min) { }
        public MinAttribute(string min) : this(typeof(string), min) { }
        public MinAttribute(double min) : this(typeof(double), min) { }

        public MinAttribute(Type _type, object min):base(_type,min,null)
        {
        }
    }
}
