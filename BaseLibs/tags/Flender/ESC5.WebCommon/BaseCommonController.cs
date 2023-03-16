using ESC5.AppBase;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESC5.WebCommon
{
    public class BaseCommonController<TUser> : AppBaseController<TUser> where TUser : IFuncUser
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //覆盖基类方法，使得请求与_ForViewModelOnly参数无关，每个action无论是否关联视图文件，都会被执行
        }
         public ActionResult FuncList()
        {
            var html = new HtmlString(Util.FuncTree);
            return PartialView("~/Shared/Directive/FuncList.cshtml",html);
        }
        public new ActionResult AuthFailure()
        {
            return base.AuthFailure();
        }
        public ActionResult NgControllerJs()
        {
            return JavaScript(Util.NgControllerJs());
        }
        public ActionResult CheckUnique(string name, string value)
        {
            return RcJson(name==value);
        }
    }
}
