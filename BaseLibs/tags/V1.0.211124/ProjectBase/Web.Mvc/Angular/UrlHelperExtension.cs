using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
            if (ApplicationPath.Length > 1 && stateName.StartsWith(ApplicationPath))
            {
                stateName = stateName.Substring(ApplicationPath.Length );
            }
            return stateName;
        }

        public static String State(this IUrlHelper urlHelper,String sref)
        {
            return State(urlHelper, sref, null /* controllerName */, null);
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
