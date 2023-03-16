using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectBase.Utils;

namespace ProjectBase.Web.Mvc.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class VmInnerAttribute : GroupedContextValidationAttribute
    {
        //public Type InnerValidatorClass { get; set; }
        public string ValMethod { get; set; }

        public VmInnerAttribute(string valMethod)
        {
            ValidationType = GlobalConstant.ValidationType_VmInnerValPrefix + valMethod;
            ValMethod = valMethod;
        }
        public VmInnerAttribute(string valMethod, string groups)
        {
            ValidationType = GlobalConstant.ValidationType_VmInnerValPrefix + valMethod;
            ValMethod = valMethod;
            Groups = groups;
        }
        public override IEnumerable<BaseModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata)
        {
            var rule = new BaseModelClientValidationRule(ValidationType, null, GroupArray, false);
            return new List<BaseModelClientValidationRule> {
                    rule
                };
        }

        protected override bool IsValid(ValidationContext validationContext, object value)
        {
            bool? isValid = false;
            try
            {
                isValid = validationContext.ObjectType.GetMethod(ValMethod).Invoke(validationContext.ObjectInstance, null) as bool?;
            }
            catch (Exception e)
            {
                throw new NetArchException("执行VM内部验证方法时异常，请检查方法存在且返回bool型值：" + value.GetType() + "." + ValMethod + "\r\n" + e);
            }
            if (isValid == null)
            {
                throw new NetArchException("VM内部验证方法不可返回null：" + value.GetType() + "." + ValMethod);
            }
            return isValid.Value;
        }

    }
}
