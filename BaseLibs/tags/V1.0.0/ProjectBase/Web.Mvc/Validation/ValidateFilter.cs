using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ProjectBase.Web.Mvc.Validation
{
      public class ValidateFilter : IAuthorizationFilter,IActionFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var t = filterContext.ModelState;
            var pds = ((ControllerActionDescriptor)filterContext.ActionDescriptor).MethodInfo.GetParameters();
            foreach (var pd in pds)
            {
                var valAttr = pd.GetCustomAttributes(false).OfType<ValidateAttribute>().SingleOrDefault();
                if (valAttr == null) continue;

                var dict = (IDictionary<Type, string[]>)filterContext.HttpContext.Items[GlobalConstant.Request_Attr_ValGroups];
                if (dict == null)
                {
                    dict = new Dictionary<Type, string[]>();
                }
                Type tobeVal = valAttr.TargetClass;
                if (tobeVal == null) tobeVal = pd.ParameterType;
                dict.Add(tobeVal, valAttr.GroupArray);
                filterContext.HttpContext.Items[GlobalConstant.Request_Attr_ValGroups] = dict;
            }
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Items[GlobalConstant.Request_Attr_ValGroups] != null
                  && !filterContext.ModelState.IsValid)
            {
                filterContext.Result = ((BaseController)filterContext.Controller).ClientShowMessage();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}