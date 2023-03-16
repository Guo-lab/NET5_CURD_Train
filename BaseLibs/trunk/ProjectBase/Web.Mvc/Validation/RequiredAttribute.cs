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
    public class RequiredAttribute : GroupedValidationAttribute
    {
        public RequiredAttribute()
        {
            ValidationType = "pb-Grequired";
        }
        public override bool IsValid(object value)
        {
            return value != null;
        }
        public override IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(
                ModelMetadata metadata)
        {
            var rule = new BaseModelClientValidationRule(ValidationType, ErrorMessage, GroupArray, false);
            return new List<BaseModelClientValidationRule> { rule };
        }
    }
}
