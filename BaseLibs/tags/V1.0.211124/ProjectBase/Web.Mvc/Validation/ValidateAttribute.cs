using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ProjectBase.Web.Mvc.Validation
{
    /// <summary>
    /// 分组验证。
    /// 在Action的参数上加标记以便进行分组验证。
    /// 在AuthorizationFilter阶段，尚未进行ModelBinding，此时进行验证分组标记的分析。
    ///   当进行了验证后自动返回错误，不进入Action方法。
    ///   see also:ValidateFilter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ValidateAttribute : Attribute
    {
        public string[] GroupArray { get; set; }
        public string Groups
        {
            get
            {
                return GroupArray==null?null: GroupArray.Aggregate((a, b) => a + "," + b);
            }
            set
            {
                if (value != null)
                {
                    GroupArray = value.Split(',');
                }
                else
                {
                    GroupArray = null;
                }
            }
        }
        public string ErrorMessage { get; set; }
        //要验证的类。缺省为标记所在的参数的类以及其所有内部类。
        public Type TargetClass { get; set; }

        public ValidateAttribute()
        {
            Groups = null;
            ErrorMessage = "GroupedValidation_ValidateAllFailed";
        }

        public ValidateAttribute(string groups)
        {
            Groups = groups;
            ErrorMessage = "";
        }

        public ValidateAttribute(string groups, string errorMessage)
        {
            Groups = groups;
            ErrorMessage = "GroupedValidation_ValidateGroupsFailed";
        }

    }
}