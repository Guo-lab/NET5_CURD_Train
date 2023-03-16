using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBase.Web.Mvc;

namespace ESC5.Web.Mvc.Shared
{
    /// <summary>
    /// 因为本应用与登录无关，此处不继承ESC5.WebCommon.AppBaseController<TUser>,所以直接继承BaseController
    /// </summary>
    public class CommonController:BaseController
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //覆盖基类方法，使得请求与_ForViewModelOnly参数无关，每个action无论是否关联视图文件，都会被执行
        }
        public ActionResult NgControllerJs()
        {
            return JavaScript(Util.NgControllerJs());
        }
    }
}
