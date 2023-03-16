using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ProjectBase.Web.Mvc
{


    //防止客户端重复点击造成重复记录
    public class PreventDuplicateAttribute:ActionFilterAttribute
    {
        //缺省情况下10s内不能重复请求，可通过构造函数传递其它间隔
        private int _delayRequest;
        public PreventDuplicateAttribute(int delayRequest)
        {
            this._delayRequest = delayRequest;
        }
        public PreventDuplicateAttribute():this(10)
        {
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                DuplicateRequestValidator.Verify(_delayRequest, filterContext);
                base.OnActionExecuting(filterContext);
            }
            catch (Exception e)
            {
                var controller = (BaseController)filterContext.Controller;
                if (e is BusinessDelegate.BizException)
                {
                    filterContext.Result = controller.RcJsonError("BizException", ((BusinessDelegate.BizException)e).ExceptionKey);
                }
                else if (e is NHibernate.StaleStateException)
                {
                    filterContext.Result = controller.RcJsonError("StaleException");
                }
            }
        }
    }
}
