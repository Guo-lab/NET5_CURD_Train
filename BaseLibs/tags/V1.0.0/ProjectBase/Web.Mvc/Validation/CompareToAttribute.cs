using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectBase.Utils;

namespace ProjectBase.Web.Mvc.Validation
{
    public class CompareToAttribute : GroupedContextValidationAttribute
    {
        public enum CompareEnum
        {
            Equal = 0,
            MoreThan = 1,
            LessThan = -1,
            MoreThanEqual = 10,
            LessThanEqual = -10
        }

        public string OtherPropName { get; set; }
        public CompareEnum Compare { get; set; } = CompareEnum.Equal;
        public CompareToAttribute(string otherPropName)
        {
            OtherPropName = otherPropName;
        }
        protected override bool IsValid(ValidationContext validationContext, object value)
        {
            if (value == null) return true;
            object otherPropValue = validationContext.ObjectType.GetProperty(OtherPropName).GetValue(validationContext.ObjectInstance);
            if (Compare == CompareEnum.Equal)
            {
                return value.Equals(otherPropValue);
            }
            if (typeof(int).IsAssignableFrom(value.GetType()))
            {
                int intValue=(int)value; 
                switch (Compare)
                {
                    case CompareEnum.MoreThan:
                        return intValue > (int)otherPropValue;
                    case CompareEnum.LessThan:
                        return intValue < (int)otherPropValue;
                    case CompareEnum.MoreThanEqual:
                        return intValue >= (int)otherPropValue;
                    case CompareEnum.LessThanEqual:
                        return intValue <= (int)otherPropValue;
                }
            }
            if (value is DateTime time)
            {
                switch (Compare) {
                    case CompareEnum.MoreThan:
                        return time > (DateTime)otherPropValue;
                    case CompareEnum.LessThan:
                        return time < (DateTime)otherPropValue;
                    case CompareEnum.MoreThanEqual:
                        return time >= (DateTime)otherPropValue;
                    case CompareEnum.LessThanEqual:
                        return time <= (DateTime)otherPropValue;
                }
            }
            if (value is IComparable comparable)
            {
                switch (Compare)
                {
                    case CompareEnum.MoreThan:
                        return comparable.CompareTo(otherPropValue) > 0;
                    case CompareEnum.LessThan:
                        return comparable.CompareTo(otherPropValue) < 0;
                    case CompareEnum.MoreThanEqual:
                        return comparable.CompareTo(otherPropValue) >= 0;
                    case CompareEnum.LessThanEqual:
                        return comparable.CompareTo(otherPropValue) <= 0;
                }
            }
            throw new NetArchException("暂不支持这种类型的比较");

        }

        public override IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata)
        {
            var rules = new List<BaseModelClientValidationRule>();
            BaseModelClientValidationRule rule;
            
            rule = new BaseModelClientValidationRule("pb-Compareto", ErrorMessage, GroupArray, true);
            rule.ValidationParameters["other"] = ClientPropPathPrefix+OtherPropName;
            rule.ValidationParameters["compare"] = Compare;
            rule.ValidationParameters["isDate"] = typeof(DateTime).IsAssignableFrom(metadata.ModelType);
            rules.Add(rule);

            return rules;
        }
    }
}
