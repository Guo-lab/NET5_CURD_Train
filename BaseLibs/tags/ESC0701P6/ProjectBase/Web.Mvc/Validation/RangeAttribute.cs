using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjectBase.Web.Mvc.Validation
{
    /// <summary>
    /// [Range]、[Max]、[Min]三者只能使用其中一个.
    /// 限制值应与被验证值类型一致：日期型数据用字符串限制值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited =true)]
    public class RangeAttribute : GroupedValidationAttribute
    {
        public RangeAttribute(int min, int max) : this(typeof(int), min, max) { }
        public RangeAttribute(string min, string max) : this(typeof(string), min, max) { }
        public RangeAttribute(double min, double max) : this(typeof(double), min, max) { }

        //private Type type;
        protected RangeAttribute(Type _type, object min, object max)
        {
           // type = _type;
            Max = max;
            Min = min;
        }

        public object Max { get; private set; }
        public object Min { get; private set; }

        public override bool IsValid(object value)
        {
            if (value==null) return true;
            if(value is DateTime)
            {
                Max = Max==null? null: DateTime.Parse(Max.ToString());
                Min = Min==null? null: DateTime.Parse(Min.ToString());
            }

            if (Min!=null && (value as IComparable).CompareTo(Convert.ChangeType(Min,value.GetType()))<0) return false;
            if (Max!=null)
            {
                return (value as IComparable).CompareTo(Convert.ChangeType(Max, value.GetType())) <= 0;
            }
            return true;
        }

        public override IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata)
        {
            var rules = new List<BaseModelClientValidationRule>();
            var rule = new BaseModelClientValidationRule("pb-Range", ErrorMessage, GroupArray, true);
            rule.ValidationParameters["max"] = Max;
            rule.ValidationParameters["min"] = Min;
            rules.Add(rule);
            return rules;
        }
    }
}
