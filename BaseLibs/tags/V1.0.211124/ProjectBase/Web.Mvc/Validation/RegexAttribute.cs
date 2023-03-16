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
    public class RegexAttribute : GroupedValidationAttribute
    {
        private Regex Regex;
        private string _pattern;
        private string _messageKey;

        public RegexAttribute(string pattern, string messageKey)
        {
            _pattern = pattern;
            _messageKey = messageKey;
            Regex = new Regex(pattern);
        }

        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty((string)value)) return true;
            return Regex.IsMatch((string)value);
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            base.AddValidation(context);
            MergeAttribute(context.Attributes, "pb-valmsg", "{\"pattern\":\"" + _messageKey + "\"}");
        }

        public override IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata)
        {
            var rule = new BaseModelClientValidationRule( "pbRegex", ErrorMessage, GroupArray, true );
            rule.ValidationParameters.Add("pattern", _pattern);
            return new List<BaseModelClientValidationRule> {
                    rule
                };
        }
    }
}
