using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Routing;
using ProjectBase.Web.Mvc.Validation;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace ProjectBase.Web.Mvc.Angular
{
    public static class HtmlHelperExtension
    {
        public static string Key_For_MessageClass = "Messages";
        public static string Attr_VMPrefix = "NetNgArch_Attr_VMPrefix";
        public static string DictJsName = "Dict";
        public static string UnSubmitNameMarker = "_";
        public static String vmJson(this IHtmlHelper htmlHelper)
        {
            var data = htmlHelper.ViewContext.ViewData.Model;
            return data==null?"":JsonConvert.SerializeObject(data);
        }
        private static String GetNgModel(this IHtmlHelper htmlHelper,String name) {
		    return htmlHelper.ViewContext.ViewData[Attr_VMPrefix] + "."+name;;
	    }
        public static IDictionary<string, object> AddClientValidation<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);

            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() { };
            if (!htmlAttributes.ContainsKey("ng-model")) htmlAttributes["ng-model"] = GetNgModel(htmlHelper, expresionProvider.GetExpressionText(expression));
            ModelMetadata meta = HtmlHelperUtil.MetaFromExpression(expresionProvider, htmlHelper, expression);
            if (meta == null) return htmlAttributes;
            return AddHtmlValidationAttributes(meta, htmlAttributes);
        }
        public static IDictionary<string, object> AddHtmlValidationAttributes(ModelMetadata meta, IDictionary<string, object> htmlAttributes)
        {
            var rules = HtmlHelperUtil.ToClientRules(meta);
            htmlAttributes["translatekey"] =  meta.DisplayName;//((DefaultModelMetadata)meta).DisplayMetadata.DisplayName;//
            foreach (var rule in rules)
            {
                switch (rule.ValidationType)
                {
                    case "required":
                        htmlAttributes["required"] = "required";
                        break;
                    case "min":
                        htmlAttributes["min"] = rule.ValidationParameters["min"];
                        break;
                    case "max":
                        htmlAttributes["max"] = rule.ValidationParameters["max"];
                        break;
                    case "range":
                        if (rule.ValidationParameters.ContainsKey("min"))
                        {
                            htmlAttributes["min"] = rule.ValidationParameters["min"];
                        }
                        if (rule.ValidationParameters.ContainsKey("max"))
                        {
                            htmlAttributes["max"] = rule.ValidationParameters["max"];
                        }
                        break;
                    case "number":
                        htmlAttributes["type"] = "number";
                        break;
                    case "length":
                        if (rule.ValidationParameters.ContainsKey("min"))
                        {
                            htmlAttributes["ng-minlength"] = rule.ValidationParameters["min"];
                        }
                        if (rule.ValidationParameters.ContainsKey("max"))
                        {
                            htmlAttributes["ng-maxlength"] = rule.ValidationParameters["max"];
                        }
                        break;
                    case "regexp":
                        var pattern = (string)rule.ValidationParameters["pattern"];
                        if (!pattern.StartsWith("/")) pattern = "/" + pattern + "/";
                        htmlAttributes["ng-pattern"] = pattern;
                        htmlAttributes["pb-valmsg"] = "{\"pattern\":\"" + rule.ErrorMessage + "\"}";
                        break;
                    case "email":
                        htmlAttributes["type"] = "email";
                        break;
                    case "date":
                        htmlAttributes["type"] = "date";
                        break;
}
}
            return htmlAttributes;
        }
        //public static IDictionary<string, object> AddKClientValidation<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        //{
        //    var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
        //    if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() { };
        //    if (!htmlAttributes.ContainsKey("k-ng-model")) htmlAttributes["k-ng-model"] = GetNgModel(htmlHelper, expresionProvider.GetExpressionText(expression));
        //    ModelMetadata meta = HtmlHelperUtil.MetaFromExpression(expresionProvider, htmlHelper, expression);
        //    if (meta == null) return htmlAttributes;
        //    return AddHtmlValidationAttributes(meta, htmlAttributes);
        //}

        public static IDictionary<string, object> AddNgModel(this IHtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() { };
            if (!htmlAttributes.ContainsKey("ng-model")) htmlAttributes["ng-model"] = GetNgModel(htmlHelper, name);
            return htmlAttributes;
        }
        //public static IDictionary<string, object> AddKNgModel(this IHtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
        //{
        //    if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() { };
        //    if (!htmlAttributes.ContainsKey("k-ng-model")) htmlAttributes["k-ng-model"] = GetNgModel(htmlHelper, name);
        //    return htmlAttributes;
        //}
        public static IDictionary<string, object> AddNgModel<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() { };
            if (!htmlAttributes.ContainsKey("ng-model")) htmlAttributes["ng-model"] = GetNgModel(htmlHelper, expresionProvider.GetExpressionText(expression)); 
            return htmlAttributes;
        }
        public static HtmlString AdjustName(HtmlString mvcHtmlString, IDictionary<string, object> htmlAttributes,bool ngModelExplicit)
        {
            String explicitname = htmlAttributes.ContainsKey("name")?(String)htmlAttributes["name"]:null;
            String nosubmit = explicitname == "" ? UnSubmitNameMarker : "";
            if (String.IsNullOrEmpty(explicitname) && ngModelExplicit)
            {
                String nameExpr = (String)htmlAttributes["ng-model"];
                if (nameExpr.IndexOf("[") > 0)
                {
                    String arrayName = nameExpr.Substring(0, nameExpr.IndexOf("["));
                    nameExpr = nameExpr.Replace("[", "[{{").Replace("]", "|ReIndex:" + arrayName + "}}]");
                }
                String nameprefix = htmlAttributes.ContainsKey("ng-name-prefix") ? (String)htmlAttributes["ng-name-prefix"] : null;
                if (nameprefix == null) nameprefix = "c.vm";
                if (!nameprefix.EndsWith(".")) nameprefix = nameprefix + ".";
                explicitname=nosubmit + nameExpr.Substring(nameprefix.Length);
            };
            var h = mvcHtmlString.Value;
            int pos0 = h.IndexOf(" id=\"");
            int pos1 = h.IndexOf("\"", pos0+5);
            h = h.Remove(pos0, pos1 - pos0+1);
            if (!String.IsNullOrEmpty(explicitname))
            {
                pos0 = h.IndexOf(" name=\"");
                pos1 = h.IndexOf("\"", pos0 + 7);
                h = h.Remove(pos0, pos1 - pos0 + 1);
                h = h.Insert(pos0, " name=\"" + explicitname + "\"");
            }
            return new HtmlString(h);
        }
        public static HtmlString AdjustName(IHtmlContent hc, IDictionary<string, object> htmlAttributes, bool ngModelExplicit)
        {
            return AdjustName(HtmlHelperUtil.ToHtmlString(hc), htmlAttributes, ngModelExplicit);
        }

         public static MvcForm NgForm(this IHtmlHelper htmlHelper, string name)
        {
            return NgForm(htmlHelper, name, null, null,  FormMethod.Post, null);
        }
        public static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName)
        {
            return NgForm(htmlHelper, name, actionName, null,  FormMethod.Post, null);
        }
        public static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass)
        {
            return NgForm(htmlHelper, name, actionName, cssClass,  FormMethod.Post, null);
        }
        public static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass, object htmlAttributes)
        {
            return NgForm(htmlHelper, name, actionName, cssClass, null, htmlAttributes);
        }
        private static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass, FormMethod? method, object htmlAttributes)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (method == null && !attrs.ContainsKey("method")) 
                method = FormMethod.Post;
            else if (method == null)
            {
                method = (FormMethod)attrs["method"];
                //method = attrs["method");
            }
            if (!string.IsNullOrEmpty(cssClass)) attrs["class"] = cssClass;
            //attrs["ng-submit"] = "validate($event)";
            string controllerName = null;
            if (actionName != null && actionName.Contains("/"))
            {
                var ac = actionName.Split('/');
                actionName = ac[0];
                controllerName = ac[1];
            }
            var routeValue = htmlHelper.ViewContext.RouteData.Values;
            var area = routeValue["area"] == null ? "" : "/" + routeValue["area"];
            var url = actionName == null ? null : area + "/" + (controllerName ?? routeValue["Controller"]) + "/" + actionName;
            if (!String.IsNullOrEmpty(url)) attrs["ajax-url"] = url;
            if(string.IsNullOrEmpty(name))
                name = (String) attrs["name"];
            else
            {
                attrs["name"] = name;
            }
            var s = String.IsNullOrEmpty(name) ? "" : (name.Contains(".") ? name.Split('.')[0] : "");
            htmlHelper.ViewContext.ViewData[Attr_VMPrefix]= (String.IsNullOrEmpty(s) ? "c" : s)+".vm";
            return HtmlHelperFormExtensions.BeginForm(htmlHelper,actionName, controllerName, method.Value, attrs);
        }

    }

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
            var validatorMetadata = meta.ValidatorMetadata;
            foreach (var v in validatorMetadata)
            {
                if (v is System.ComponentModel.DataAnnotations.RequiredAttribute)
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
                    if(attr.MinimumLength>0) rule.ValidationParameters.Add("min", attr.MinimumLength);
                    if (attr.MaximumLength > 0) rule.ValidationParameters.Add("max", attr.MaximumLength);
                    rules.Add(rule);
                }
                //其它自定义验证标记的客户端输出在自定义类中实现
            }
            return rules;
        }
    }
}
