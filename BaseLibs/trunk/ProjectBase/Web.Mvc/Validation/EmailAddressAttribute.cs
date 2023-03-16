using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProjectBase.Web.Mvc.Validation
{
    public class EmailAddressAttribute : GroupedValidationAttribute
    {
        private static readonly Regex emailRegex = new Regex("^\\w+((-\\w+)|(.\\w+))*@[A-Za-z0-9]+((.|-)[A-Za-z0-9]+)*.[A-Za-z0-9]+$");

        public EmailAddressAttribute()
        {
            ValidationType = "pattern";
        }
        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty((string)value)) return true;
            string[] aEmail = value.ToString().Split(',');
            foreach (string sEmail in aEmail)
            {
                if (!emailRegex.IsMatch(sEmail))
                    return false;
            }
            return true;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (!ShouldAddValidation(context)) return;
            base.AddValidation(context);
            MergeAttribute(context.Attributes, "pb-valmsg", "{\"pattern\":\"InvalidEmailFormat\"}");
        }
        public override IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata)
        {
            var rule = new BaseModelClientValidationRule(ValidationType, ErrorMessage, GroupArray, true);
            rule.ValidationParameters["pattern"] = "(\\w+((-\\w+)|(.\\w+))*@[A-Za-z0-9]+((.|-)[A-Za-z0-9]+)*.[A-Za-z0-9]+)+(,\\w+((-\\w+)|(.\\w+))*@[A-Za-z0-9]+((.|-)[A-Za-z0-9]+)*.[A-Za-z0-9]+)*";
            return new List<BaseModelClientValidationRule> {rule};
        }
    }
}
