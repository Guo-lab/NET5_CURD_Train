using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace ProjectBase.Web.Mvc.Validation
{
    public abstract class GroupedValidationAttribute : ValidationAttribute, IClientValidatable,IClientModelValidator
    {
        public string[] GroupArray { get; set; }
        // 所属的验证分组
        public string Groups {
            get {
                return GroupArray.Aggregate( ( a, b ) => a + "," + b );
            }
            set {
                if (value != null)
                {
                    GroupArray = value.Split( ',' );
                }
            }
        }

        public abstract override bool IsValid ( object value );

        public abstract IEnumerable<BaseModelClientValidationRule> GetClientValidationRules (ModelMetadata metadata );

        protected override ValidationResult IsValid ( object value, ValidationContext validationContext )
        {
            return ShouldVal( validationContext ) && !IsValid ( value )  ?base.IsValid( value, validationContext ):ValidationResult.Success;
        }

        protected bool ShouldVal ( ValidationContext validationContext )
        {
            var hca = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var dict= (IDictionary<Type, string[]>)hca.HttpContext.Items[GlobalConstant.Request_Attr_ValGroups];
            if (dict == null && !(validationContext.ObjectInstance is IValidateWhen)) return true;

            string[] grps=null;
            if (dict != null)
            {
                Type tobeVal = validationContext.ObjectType;
                while (true)
                {
                    dict.TryGetValue(tobeVal, out grps);
                    if (grps == null && tobeVal.DeclaringType != null)
                    {
                        tobeVal = tobeVal.DeclaringType;
                    }
                    else
                    {
                        break;
                    }
                }
            } 
            if (grps == null && validationContext.ObjectInstance is IValidateWhen)
            {//[Validate]中没指定要验证的组，可能在IValidateWhen中指定
                var model = validationContext.ObjectInstance as IValidateWhen;
                grps = model.ShoulValGroups(validationContext)?.Split(',');
                if (grps == null) return false;
            }
            var defaultGrps= new string[] { GlobalConstant.Default_ValGroup };
            var myGroupArray = GroupArray ?? defaultGrps;
            if (grps == null || grps.Length == 0)
            {
                grps = defaultGrps;
            }
            return (grps.Intersect(myGroupArray).Count() > 0);
        }

        public virtual void AddValidation(ClientModelValidationContext context)
        {
            var rules = this.GetClientValidationRules(context.ModelMetadata);
            foreach(var rule in rules)
            {
                string name = rule.ValidationType;
                if (rule.ValidationParameters == null || rule.ValidationParameters.Count == 0)
                {
                    if (rule.ValidationType.StartsWith(GlobalConstant.ValidationType_VmInnerValPrefix))
                    {
                        name = GlobalConstant.ValidationType_VmInnerValPrefix;
                        MergeAttribute(context.Attributes, name, rule.ValidationType.Substring(GlobalConstant.ValidationType_VmInnerValPrefix.Length));
                    }
                    else
                    {
                        MergeAttribute(context.Attributes, rule.ValidationType, "");
                    }
                }
                else
                {
                    MergeAttribute(context.Attributes, rule.ValidationType, rule.ValidationParameters.First().Value?.ToString());
                    var ruleparams=JsonConvert.SerializeObject(rule.ValidationParameters);
                    MergeAttribute(context.Attributes, rule.ValidationType + "-params" , ruleparams);
                }
                if (rule.Groups != null && rule.Groups.Count() > 0)
                {
                    MergeAttribute(context.Attributes, name + "-groups", rule.Groups.Aggregate((a,i)=>a=a+","+i));
                }
            }
        }
        protected static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            attributes[key]=value;
        }
    }
}
