using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProjectBase.Web.Mvc.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc.Angular
{
    public class HtmlHelperUtil
    {
        public static ModelExpressionProvider GetExpressionProvider<TModel>(IHtmlHelper<TModel> htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
        }
        public static ModelMetadata MetaFromExpression<TModel, TProperty>(ModelExpressionProvider expresionProvider, IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return expresionProvider.CreateModelExpression(htmlHelper.ViewData, expression).Metadata;
        }
        public static string ToString(IHtmlContent hc)
        {
            var result = "";
            using (var sw = new System.IO.StringWriter())
            {
                hc.WriteTo(sw, System.Text.Encodings.Web.HtmlEncoder.Default);
                result = sw.ToString();
            }
            return result;
        }
        public static HtmlString ToHtmlString(IHtmlContent hc)
        {
            return new HtmlString(ToString(hc));
        }
        /// <summary>
        /// 因为System.ComponentModel.DataAnnotations下的标记对应的客户端输出不宜用子类改写，因此在此处转换
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public static IEnumerable<BaseModelClientValidationRule> ToClientRules(ModelMetadata meta)
        {
            var rules = new List<BaseModelClientValidationRule>();
            if (meta.IsRequired)//kendo控件无法通过ClientDataTypeModelValidatorProvider来自动加上required，因此在这里加
            {
                rules.Add(new BaseModelClientValidationRule("required", false));
            }
            var validatorMetadata = meta.ValidatorMetadata;
            foreach (var v in validatorMetadata)
            {
                if (!meta.IsRequired && v is System.ComponentModel.DataAnnotations.RequiredAttribute)
                {
                    rules.Add(new BaseModelClientValidationRule("required", false));
                }
                else if (v is System.ComponentModel.DataAnnotations.RangeAttribute)
                {
                    var attr = ((System.ComponentModel.DataAnnotations.RangeAttribute)v);
                    var rule = new BaseModelClientValidationRule("min", true);
                    rule.ValidationParameters.Add("min", attr.Minimum);
                    rules.Add(rule);
                    rule = new BaseModelClientValidationRule("max", true);
                    rule.ValidationParameters.Add("max", attr.Maximum);
                    rules.Add(rule);
                }
                else if (v is System.ComponentModel.DataAnnotations.EmailAddressAttribute)
                {
                    rules.Add(new BaseModelClientValidationRule("email", false));
                }
                else if (v is System.ComponentModel.DataAnnotations.StringLengthAttribute)
                {
                    var attr = ((System.ComponentModel.DataAnnotations.StringLengthAttribute)v);
                    var rule = new BaseModelClientValidationRule("length", true);
                    if (attr.MinimumLength > 0) rule.ValidationParameters.Add("min", attr.MinimumLength);
                    if (attr.MaximumLength > 0) rule.ValidationParameters.Add("max", attr.MaximumLength);
                    rules.Add(rule);
                }
                //其它自定义验证标记的客户端输出在自定义类中实现
            }
            return rules;
        }
    }
}
