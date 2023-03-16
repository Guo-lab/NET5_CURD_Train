using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ProjectBase.Web.Mvc
{
    [Obsolete("不要使用，将马上被删除")]
    public class ThrowableAttribute : ActionFilterAttribute
    {



        /// <summary>
        ///     When used, assumes the <see cref = "factoryKey" /> to be NHibernateSession.DefaultFactoryKey
        /// </summary>
        public ThrowableAttribute()
        {
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            try
            {
                if (filterContext.Exception != null)
                {
                    ShowUserError(filterContext, filterContext.Exception);
                }
            }
            catch (Exception e)
            {
                if (!ShowUserError(filterContext, e))
                    throw;
            }

        }

        private bool ShowUserError(ActionExecutedContext filterContext, Exception e)
        {
            var controller = (BaseController)filterContext.Controller;
            if (e is BusinessDelegate.BizException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = controller.RcJsonError("BizException", ((BusinessDelegate.BizException)e).ExceptionKey);
            }
            
            return false;
        }
    }
}