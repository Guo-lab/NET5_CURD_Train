using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjectBase.Web.Mvc.Validation
{
    public class StringLengthAttribute : GroupedValidationAttribute
    {
        private int maximumLength;

        public int MaximumLength { get { return maximumLength; } }
        public int MinimumLength { get; set; } = -1;
        public StringLengthAttribute(int maximumLength)
        {
            ValidationType = "pb-String-Length";
            this.maximumLength = maximumLength;
        }
        public override bool IsValid(object value)
        {
            if (value==null) return true;
            var ok = true;
            if (maximumLength > 0) ok = (value as string).Length <= maximumLength;
            if (!ok) return false;
            if (MinimumLength > -1)
            {
                return (value as string).Length >= MinimumLength;
            }
            return true;
        }

        public override IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata)
        {
            var rules = new List<BaseModelClientValidationRule>();
            var rule = new BaseModelClientValidationRule(ValidationType, ErrorMessage, GroupArray, true);
            rule.ValidationParameters["max"] = MaximumLength;
            rule.ValidationParameters["min"] = MinimumLength;
            rules.Add(rule);
            return rules;
        }
    }
}
