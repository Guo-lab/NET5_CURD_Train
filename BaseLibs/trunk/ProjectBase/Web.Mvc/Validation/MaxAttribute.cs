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
    public class MaxAttribute : RangeAttribute
    {
        public MaxAttribute(int max) : this(typeof(int), max) { }
        public MaxAttribute(string max) : this(typeof(string), max) { }
        public MaxAttribute(double max) : this(typeof(double), max) { }

        public MaxAttribute(Type _type,  object max) : base(_type, null, max)
        {
        }
    }
}
