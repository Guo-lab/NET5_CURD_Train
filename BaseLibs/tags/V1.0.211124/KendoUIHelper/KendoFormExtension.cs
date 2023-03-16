using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;

namespace KendoUIHelper
{
    public static class KendoFormExtension
    {
        private static string Attr_VMPrefix = "NetNgArch_Attr_VMPrefix";
        public static MvcForm KendoForm(this IHtmlHelper htmlHelper, string name, string actionName, object htmlAttributes=null)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
           
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
