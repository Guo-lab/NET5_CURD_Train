using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;

namespace ProjectBase.Web.Mvc.Angular
{
    public static class UrlHelperExtension
    {
        public static string ApplicationPath { get; set; }
        public static string PathBase(this IUrlHelper urlHelper)
        {
            return ApplicationPath;
        }
        public static string Deprefix(string stateName)
        {
            if (ApplicationPath.Length > 1 && stateName.StartsWith(ApplicationPath,true,null)) //防止用户访问站点时的URL与IIS配置的大小写不一致
            {
                stateName = stateName.Substring(ApplicationPath.Length );
            }
            return stateName;
        }

        //暂时只加了一个有raw参数的方法，其它似乎从未用到，所以先不加
        public static IHtmlContent State(this IUrlHelper urlHelper,String sref, bool raw=true)
        {
            var s = State(urlHelper, sref, null /* controllerName */, null);
            var helper = (IHtmlHelper)urlHelper.ActionContext.HttpContext.RequestServices.GetService(typeof(IHtmlHelper));

            return raw ? helper.Raw(s) : new StringHtmlContent(s);
        }

        public static String State(this IUrlHelper urlHelper, String sref, Object routeValues)
        {
            return State(urlHelper, sref, null /* controllerName */, routeValues);
        }

        public static String State(this IUrlHelper urlHelper, String sref, RouteValueDictionary routeValues)
        {
            return State(urlHelper, sref, null /* controllerName */, routeValues);
        }

        public static String State(this IUrlHelper urlHelper, String sref, String controllerName)
        {
            return State(urlHelper, sref, controllerName, (RouteValueDictionary)null /* routeValues */);
        }
        public static String State(this IUrlHelper urlHelper, String sref, String controllerName, Object routeValues)
        {
            return State(urlHelper, sref, controllerName, new RouteValueDictionary(routeValues));
        }

        public static String State(this IUrlHelper urlHelper, String sref, String controllerName, RouteValueDictionary routeValues)
        {
            String nav = null;
            String stateName = null;
            String param = "";
            int pos = 0;
            if (sref.StartsWith("root:") || sref.StartsWith("forward:"))
            {
                pos = sref.IndexOf(":");
                nav = sref.Substring(0, pos);
                sref = sref.Substring(pos + 1);
            }
            pos = sref.IndexOf("(");
            if (pos > 0)
            {
                stateName = sref.Substring(0, pos);
                param = sref.Substring(pos + 1, sref.Length - (pos + 1)-1);
            }
            else
            {
                stateName = sref;
            }
            if (!stateName.StartsWith("/"))
            {
                var ctx = new UrlActionContext();
                ctx.Action = stateName;
                ctx.Controller = controllerName;
                ctx.Values = routeValues;
                stateName = urlHelper.Action(ctx);
                stateName = Deprefix(stateName);
            }
            if (nav != null)
                param = "({" + param + (param == "" ? "" : ",") + "'ajax-nav':'" + nav + "'})";
            else if (param!="")
            {
                param = "({" + param + "})";
            }
            var attr = "ui-sref=" + stateName + param + "" + " ui-sref-opts={reload:true,inherit:false}";
            return attr;
        }

    }
}
