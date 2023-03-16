using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjectBase.Web.Mvc.Validation
{
    public class RangeAttribute : GroupedValidationAttribute
    {
        public RangeAttribute(double min, double max) : this(typeof(double), min, max) { }

        public RangeAttribute(int min, int max) : this(typeof(int), min, max) { }
        public RangeAttribute(string min, string max) : this(typeof(string), min, max) { }

        private Type type;
        public RangeAttribute(Type _type, object min, object max)
        {
            type = _type;
            Max = max;
            Min = min;
        }

        public object Max { get; }
        public object Min { get; }

        public override bool IsValid(object value)
        {
            if (value==null) return true;
            var ok = true;
            if (Max!=null) ok = (value as IComparable).CompareTo(Max)<=0;
            if (!ok) return false;
            if (Min!=null)
            {
                return (value as IComparable).CompareTo(Min) >= 0;
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
