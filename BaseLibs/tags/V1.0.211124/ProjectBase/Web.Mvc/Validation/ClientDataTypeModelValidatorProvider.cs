using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;

namespace ProjectBase.Web.Mvc.Validation
{
    public class ClientDataTypeModelValidatorProvider : IClientModelValidatorProvider
    {

        private static readonly HashSet<Type> _numericTypes = new HashSet<Type>(new Type[] {
        typeof(byte), typeof(sbyte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(decimal)
        });
        private static readonly HashSet<Type> _integralTypes = new HashSet<Type>(new Type[] {
        typeof(byte), typeof(sbyte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong)
        });

        public static int DefaultDecimalPrecision = 18;
        public static int DefaultDecimalScale = 2;

        public IEnumerable<IClientModelValidator> GetValidators(ModelMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            return GetValidatorsImpl(metadata);
        }

        private static IEnumerable<IClientModelValidator> GetValidatorsImpl(ModelMetadata metadata)
        {
            Type type = metadata.ModelType;
            if (IsNumericType(type))
            {
                yield return new NumericModelValidator();
            }
            if ((metadata.ModelType == typeof(DateTime) || metadata.ModelType == typeof(DateTime?)))
            {
                yield return new DateTimeModelValidator();
            }
            if ((metadata.ModelType == typeof(string)))
            {
                yield return new StringModelValidator();
            }
        }

        public static bool IsNumericType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type); // strip off the Nullable<>
            return _numericTypes.Contains(underlyingType ?? type);
        }
        private static bool IsIntegralType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type); // strip off the Nullable<>
            return _integralTypes.Contains(underlyingType ?? type);
        }

        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var validator=GetValidatorsImpl(context.ModelMetadata);
            var results = context.Results;
            var resultsCount = results.Count;
            for (var i = 0; i < resultsCount; i++)
            {
                var v = results[i].Validator;
                if (v != null && v.GetType()==validator.GetType())
                {
                    // A validator is already present. No need to add one.
                    return;
                }
            }

            if (validator.Count()>0) {
                results.Add(new ClientValidatorItem
                {
                    Validator = validator.Single(),
                    IsReusable = true
                });
            }
        }
        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
                attributes[key]=value;
        }
        internal sealed class NumericModelValidator : IClientValidatable, IClientModelValidator
        {
            public void AddValidation(ClientModelValidationContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }
                var rules=GetClientValidationRules(context.ModelMetadata);
                foreach (var rule in rules)
                {
                    switch (rule.ValidationType)
                    {
                        case "pb-Grequired":
                            MergeAttribute(context.Attributes, "pb-Grequired", "");
                            break;
                        case "number":
                            MergeAttribute(context.Attributes, "type", "number");
                            break;
                        case "min":
                            MergeAttribute(context.Attributes, "min", rule.ValidationParameters["min"].ToString());
                            break;
                        case "max":

                            MergeAttribute(context.Attributes, "max", rule.ValidationParameters["max"].ToString());
                            break;
                        case "pattern":
                            MergeAttribute(context.Attributes, "ng-pattern", (string)rule.ValidationParameters["pattern"]);
                            MergeAttribute(context.Attributes, "pb-valmsg", "{\"pattern\":\"" + rule.ErrorMessage + "\"}");
                            break;
                    }
                }
            }


            public IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata _metadata)
            {
                BaseModelClientValidationRule rule;
                BaseModelClientValidationRule rule2;
                long min = long.MinValue, max = long.MaxValue;

                var rules=new List<BaseModelClientValidationRule>();
                if (Nullable.GetUnderlyingType(_metadata.ModelType)==null) {
                    rules.Add(new BaseModelClientValidationRule("pb-Grequired", false));
                }
                var rule0 = new BaseModelClientValidationRule("number",false);
                if (_metadata.ContainerType != null)
                {

                        //add default range for integral number
                    if (IsIntegralType(_metadata.ModelType)
                        &&
                        !Attribute.IsDefined(_metadata.ContainerType.GetProperty(_metadata.PropertyName),
                                             typeof(RangeAttribute), true))
                    {
                        if (_metadata.ModelType == typeof(long))
                        {
                            min = long.MinValue;
                            max = long.MaxValue;
                        }
                        else if (_metadata.ModelType == typeof(int))
                        {
                            min = int.MinValue;
                            max = int.MaxValue;
                        }
                        else if (_metadata.ModelType == typeof(short))
                        {
                            min = short.MinValue;
                            max = short.MaxValue;
                        }

                        rule = new BaseModelClientValidationRule("min", true);
                        rule.ValidationParameters["min"] = min;
                        rule2 = new BaseModelClientValidationRule("max", true);
                        rule2.ValidationParameters["max"] = max;

                        rules.AddRange(new[] { rule0, rule, rule2 });
                        return rules;
                    }


                    //add default edit format for decimal
                    if (_metadata.ModelType == typeof(decimal) || _metadata.ModelType == typeof(decimal?))
                    {
                        var precision = DefaultDecimalPrecision;
                        var scale = DefaultDecimalScale;

                        var attr =
                            Attribute.GetCustomAttribute(
                                _metadata.ContainerType.GetProperty(_metadata.PropertyName),
                                typeof(DecimalFormatAttribute), true) as DecimalFormatAttribute;
                        if (attr != null)
                        {
                            precision = attr.Precision;
                            scale = attr.Scale;
                        }

                        // var rex = "^(\\d{1," + (precision - scale) + "})(\\.\\d{1," + scale + "})?$";
                        var rex = "^(\\d{1," + (precision - scale) + "})";
                        if (scale > 0) rex = rex + "(\\.\\d{1," + scale + "})?";
                        rex = rex + "$";
                        rule = new BaseModelClientValidationRule("pattern", true);
                        rule.ValidationParameters.Add("pattern", rex);

                        rules.AddRange(new[] { rule0, rule });
                        return rules;
                    }
                }

                rules.Add( rule0 );
                return rules;
            }
        }

        internal sealed class DateTimeModelValidator : IClientValidatable, IClientModelValidator
        {
            public void AddValidation(ClientModelValidationContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                MergeAttribute(context.Attributes, "type", "date");
            }

            public IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata _metadata)
            {
                var rule = new BaseModelClientValidationRule("date", false);
                return new[] { rule };
            }

        }
        internal sealed class StringModelValidator : IClientValidatable, IClientModelValidator
        {
            public void AddValidation(ClientModelValidationContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }
                var rules = GetClientValidationRules(context.ModelMetadata);
                if(rules.Count()>0)
                {
                    var rule = rules.First();
                    MergeAttribute(context.Attributes, "ng-pattern", (string)rule.ValidationParameters["pattern"]);
                    MergeAttribute(context.Attributes, "pb-valmsg", "{\"pattern\":\"" + rule.ErrorMessage + "\"}");
                }
            }

            public IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata _metadata)
            {
                BaseModelClientValidationRule rule = null;

                if (_metadata.ContainerType != null)
                {
                    //add default edit format for decimal string
                    var attr =
                        Attribute.GetCustomAttribute(
                            _metadata.ContainerType.GetProperty(_metadata.PropertyName),
                            typeof(DecimalFormatAttribute), true) as DecimalFormatAttribute;
                    if (attr != null)
                    {
                        var precision = attr.Precision;
                        var scale = attr.Scale;
                        var rex = "^(\\d{1," + (precision - scale) + "})";
                        if (scale > 0) rex = rex + "(\\.\\d{1," + scale + "})?";
                        rex = rex + "$";
                        rule = new BaseModelClientValidationRule("pattern", true);
                        rule.ValidationParameters["pattern"] = rex;
                    }
                }

                if (rule == null)
                    return new BaseModelClientValidationRule[] { };
                else
                    return new[] { rule };
            }
        }
        public static List<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            var rules= new List<BaseModelClientValidationRule>();
            var validators= GetValidatorsImpl(metadata);
            foreach (var validator in validators)
            {
                rules.AddRange(((IClientValidatable)validator).GetClientValidationRules(metadata));
            }
            return rules;
        }

    }

}

