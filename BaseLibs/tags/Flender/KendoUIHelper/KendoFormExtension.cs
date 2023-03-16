using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using ProjectBase.Web.Mvc.Angular;
using System;
using System.Collections.Generic;

namespace KendoUIHelper
{
    public static class KendoFormExtension
    {
        private static string Attr_VMPrefix = "NetNgArch_Attr_VMPrefix";
        public static MvcForm KendoForm(this IHtmlHelper htmlHelper, string name, string actionName, object htmlAttributes = null)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return KendoFormInternal(htmlHelper, name, actionName, attrs);
        }
        public static MvcForm KendoFormArray(this IHtmlHelper htmlHelper, string name, string actionName, object htmlAttributes = null)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //由于表单数组name值不能含绑定表达式，只能使用固定值，因此使用pbFormArrayIndex指令记录其index以便区别
            attrs["pb-form-array-index"] = "{{$index}}";
            HtmlHelperExtension.PrepareFormArray(htmlHelper, attrs);
            return KendoFormInternal(htmlHelper, name, actionName, attrs);
        }
        private static MvcForm KendoFormInternal(this IHtmlHelper htmlHelper, string name, string actionName, IDictionary<string, object> attrs)
        {
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
            attrs["kendo-validator"] = name;
            var s = String.IsNullOrEmpty(name) ? "" : (name.Contains(".") ? name.Split('.')[0] : "");
            htmlHelper.ViewContext.ViewData[Attr_VMPrefix] = (String.IsNullOrEmpty(s) ? "c" : s) + ".vm";
            return HtmlHelperFormExtensions.BeginForm(htmlHelper, actionName, controllerName, FormMethod.Post, attrs);
        }
    }
}
