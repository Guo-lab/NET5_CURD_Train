using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc.Angular;


namespace ProjectBase.Web.Mvc
{
    public  class BaseAuthAttribute : Attribute
    {
        public string FuncCode { get; set; }
        public bool AuthEnabled { get; set; }
        public bool CheckLogin { get; set; } = true;

        private bool _forAllActions = false;
        [Obsolete("即将删除")]
        public bool ForAllActions { get {
                return _forAllActions;
            } set {
               //// if (value == false) throw new NetArchException("只能设置ForAllActions为true,即[Auth(ForAllActions=true)]");
                _forAllActions = value;
            } }

        protected BaseAuthAttribute()
        {
            AuthEnabled = true;
        }

        protected BaseAuthAttribute(string funcCode)
        {
            AuthEnabled = true;
            FuncCode = funcCode;
        }
        public BaseAuthAttribute(bool authEnabled)
        {
            AuthEnabled = authEnabled;
        }
    }
    /// <summary>
    /// check user's priviledges to accessing actions.
    /// you can assign funccode to each action,or the default value for funccode is controllername.actionname 
    /// 注意这个BaseAuthFilter被多线程共享，因此不能使用实例变量，而是放到request中
    /// </summary>
    public abstract class BaseAuthFilter<TUser, TAttribute> : AttributeParserAsFilter<TAttribute>, IAuthorizationFilter, IOrderedFilter where TAttribute : Attribute
    {
        public int Order => 0;
        protected abstract bool IsAuthEnabled(HttpContext context);
        protected abstract bool ShouldCheckLogin(HttpContext context);

        private const string FailureResult = "FailureResult";

        public virtual void OnAuthorization(AuthorizationFilterContext filterContext) {
            var descriptor = (ControllerActionDescriptor)filterContext.ActionDescriptor;
            var controllerActionName = descriptor.ControllerName + "." +descriptor.ActionName;

            var controllerAuth = GetControllerAuth(descriptor);
            var actionauth = GetActionAuth(descriptor);
            if (controllerAuth == null && actionauth == null)
            {
                return;
            }

            if (ShouldParseControllerDefault())
            {
                SetAttributeOnController(filterContext.HttpContext,controllerAuth);
            }
            SetAttribute(filterContext.HttpContext, actionauth);
            if (AuthorizeCore(filterContext.HttpContext, controllerActionName)) {

            } else {
                HandleUnauthorizedRequest(filterContext);
            }
        }

        private TAttribute GetControllerAuth(ControllerActionDescriptor descriptor)
        {
            return (TAttribute)Attribute.GetCustomAttribute(descriptor.ControllerTypeInfo, typeof(TAttribute));
        }
        private TAttribute GetActionAuth(ControllerActionDescriptor descriptor)
        {
            return (TAttribute)Attribute.GetCustomAttribute(descriptor.MethodInfo, typeof(TAttribute));
        }
        protected bool AuthorizeCore(HttpContext httpContext,string controllerActionName)
        {   
            if (!IsAuthEnabled(httpContext)) return true;
            var result = ShouldCheckLogin(httpContext) ?CheckLogin(httpContext):null;
            if (result != null){
                httpContext.Items[FailureResult] = result;
                return false;
            }

            if (CanAccess(httpContext, controllerActionName))
                WriteUserLog(httpContext);
            else
            {
                httpContext.Items[FailureResult] = new RichClientJsonResult(false, "AuthFailure", "AuthFailure"); 
                return false;
            }

            return true;
        }

        protected void HandleUnauthorizedRequest(AuthorizationFilterContext context)
        {
                context.Result = (IActionResult)context.HttpContext.Items[FailureResult];
        }

        protected virtual ActionResult CheckLogin(HttpContext httpContext)
        {
            return null;
        }

        protected abstract bool CanAccess(HttpContext httpContext, string controllerActionName);

        protected virtual void WriteUserLog(HttpContext httpContext)
        {
        }

    }
}
