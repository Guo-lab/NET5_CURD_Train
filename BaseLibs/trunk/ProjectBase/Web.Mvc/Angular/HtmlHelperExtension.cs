using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProjectBase.Web.Mvc.Validation;
using ProjectBase.Utils;

namespace ProjectBase.Web.Mvc.Angular
{
    public static class HtmlHelperExtension
    {
        public static string Key_For_MessageClass = "Messages";
        public static string Attr_VMPrefix = "NetNgArch_Attr_VMPrefix";
        public static string DictJsName = "Dict";
        public static string UnSubmitNameMarker = "_";
        public const string Key_For_FormArray_DummyPrefix = "DummyPrefix";
        public const string Key_For_FormArray_IndexedPrefix = "IndexedPrefix";
        public const string FormArray_NgModel_DefaultPrefix = "item";
        public static String vmJson(this IHtmlHelper htmlHelper)
        {
            var data = htmlHelper.ViewContext.ViewData.Model;
            return data == null ? "" : JsonConvert.SerializeObject(data);
        }
        private static String GetNgModel(this IHtmlHelper htmlHelper, String name)
        {
            return htmlHelper.ViewContext.ViewData[Attr_VMPrefix] + "." + name; ;
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
            htmlAttributes["translatekey"] = meta.DisplayName;//((DefaultModelMetadata)meta).DisplayMetadata.DisplayName;//
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
            var attr = Attribute.GetCustomAttribute(meta.ContainerType.GetProperty(meta.PropertyName), typeof(ValidateWhenAttribute)) as ValidateWhenAttribute;
            if (attr != null)
            {
                htmlAttributes["pb-Validate-When"] = "true";
            }
            return htmlAttributes;
        }

        public static IDictionary<string, object> AddNgModel(this IHtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() { };
            if (!htmlAttributes.ContainsKey("ng-model")) htmlAttributes["ng-model"] = GetNgModel(htmlHelper, name);
            return htmlAttributes;
        }
        public static IDictionary<string, object> AddNgModel<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var expresionProvider = HtmlHelperUtil.GetExpressionProvider(htmlHelper);
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>() { };
            if (!htmlAttributes.ContainsKey("ng-model")) htmlAttributes["ng-model"] = GetNgModel(htmlHelper, expresionProvider.GetExpressionText(expression));
            return htmlAttributes;
        }
        public static HtmlString AdjustName(HtmlString mvcHtmlString, IDictionary<string, object> htmlAttributes, bool ngModelExplicit)
        {
            String explicitname = htmlAttributes.ContainsKey("name") ? (String)htmlAttributes["name"] : null;
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
                explicitname = nosubmit + nameExpr.Substring(nameprefix.Length);
            };
            var h = mvcHtmlString.Value;
            int pos0 = h.IndexOf(" id=\"");
            int pos1 = h.IndexOf("\"", pos0 + 5);
            h = h.Remove(pos0, pos1 - pos0 + 1);
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
            return NgForm(htmlHelper, name, null, null, FormMethod.Post, null);
        }
        public static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName)
        {
            return NgForm(htmlHelper, name, actionName, null, FormMethod.Post, null);
        }
        public static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass)
        {
            return NgForm(htmlHelper, name, actionName, cssClass, FormMethod.Post, null);
        }
        public static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass, object htmlAttributes)
        {
            return NgForm(htmlHelper, name, actionName, cssClass, null, htmlAttributes);
        }
        private static MvcForm NgForm(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass, FormMethod? method, object htmlAttributes)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return NgFormInternal(htmlHelper, name, actionName, cssClass, method, attrs);
        }
        public static MvcForm NgFormArray(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass, FormMethod? method, object htmlAttributes)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //由于表单数组name值不能含绑定表达式，只能使用固定值，因此使用pbFormArrayIndex指令记录其index以便区别
            attrs["pb-form-array-index"] = "{{$index}}";
            PrepareFormArray(htmlHelper, attrs);
            return NgFormInternal(htmlHelper, name, actionName, cssClass, method, attrs);
        }
        private static MvcForm NgFormInternal(this IHtmlHelper htmlHelper, string name, string actionName, string cssClass, FormMethod? method, IDictionary<string, object> attrs)
        {
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
            if (string.IsNullOrEmpty(name))
                name = (String)attrs["name"];
            else
            {
                attrs["name"] = name;
            }
            var s = String.IsNullOrEmpty(name) ? "" : (name.Contains(".") ? name.Split('.')[0] : "");
            htmlHelper.ViewContext.ViewData[Attr_VMPrefix] = (String.IsNullOrEmpty(s) ? "c" : s) + ".vm";
            return HtmlHelperFormExtensions.BeginForm(htmlHelper, actionName, controllerName, method.Value, attrs);
        }

        public static void PrepareFormArray(IHtmlHelper htmlHelper, IDictionary<string, object> attrs)
        {
            if (attrs.ContainsKey(Key_For_FormArray_IndexedPrefix))
            {
                htmlHelper.ViewContext.HttpContext.Items[Key_For_FormArray_IndexedPrefix] = attrs[Key_For_FormArray_IndexedPrefix];
            }
            else
            {
                throw new Exception("必须为FormArray指定参数IndexedPrefix");
            }
            if (attrs.ContainsKey(Key_For_FormArray_DummyPrefix))
            {
                htmlHelper.ViewContext.HttpContext.Items[Key_For_FormArray_DummyPrefix] = attrs[Key_For_FormArray_DummyPrefix];
            }
            else
            {
                throw new Exception("必须为FormArray指定参数DummyPrefix");
            }
        }
        public static void AdjustNameInFormArray(IHtmlHelper htmlHelper, IDictionary<string, object> htmlAttributes)
        {
            if (!IsInFormArray(htmlHelper)) return;
            var modelName = htmlAttributes["ng-model"].ToString();
            var items = htmlHelper.ViewContext.HttpContext.Items;
            var prefix = items[Key_For_FormArray_DummyPrefix].ToString();
            var fieldName = modelName.Substring(modelName.IndexOf(prefix) + prefix.Length);
            prefix = items[Key_For_FormArray_IndexedPrefix].ToString();
            htmlAttributes["ng-model"] = FormArray_NgModel_DefaultPrefix + fieldName;
            htmlAttributes["name"] = prefix + fieldName;
        }
        public static string[] KendoAdjustNameInFormArray(IHtmlHelper htmlHelper, string modelName)
        {
            var items = htmlHelper.ViewContext.HttpContext.Items;
            var prefix = items[Key_For_FormArray_DummyPrefix].ToString();
            var fieldName = modelName.Substring(modelName.IndexOf(prefix) + prefix.Length);
            prefix = items[Key_For_FormArray_IndexedPrefix].ToString();
            return new string[] { FormArray_NgModel_DefaultPrefix + fieldName, prefix + fieldName };
        }
        public static bool IsInFormArray(IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.HttpContext.Items.ContainsKey(Key_For_FormArray_IndexedPrefix);
        }
    }
}
