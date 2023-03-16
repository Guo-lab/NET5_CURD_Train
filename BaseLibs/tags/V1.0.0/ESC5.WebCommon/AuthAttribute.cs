using ESC5.AppBase;
using IdentityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using ProjectBase.Web.Mvc.Angular;
using ESC5.WebCommon;

namespace ESC5.WebCommon
{
    public class AuthAttribute : BaseAuthAttribute
    {
        public AuthAttribute()
        {
        }

        public AuthAttribute(string funcCode) : base(funcCode) { }

        public AuthAttribute(bool authEnabled) : base(authEnabled) { }
    }
    public abstract class AuthFilter<TUser> : BaseAuthFilter<TUser,AuthAttribute> where TUser : IFuncUser
    {
        private bool IsWinContext(HttpContext httpContext)
        {
            return httpContext.Request.Query.ContainsKey(RichClientJsonResult.Key_KeepDateFormat);
        }

        protected abstract int? GetDeptId(TUser? user);

        public override bool ShouldParseControllerDefault()
        {
            return true;
        }
        protected override bool IsAuthEnabled(HttpContext httpContext)
        {
            var actionAuth = GetAttribute(httpContext);
            if (actionAuth != null) return actionAuth.AuthEnabled;
            var controllerAuth = GetAttributeOnController(httpContext);
            if (controllerAuth != null) return controllerAuth.AuthEnabled;
            return false;
        }
        protected override bool ShoulCheckLogin(HttpContext httpContext)
        {
            var actionAuth = GetAttribute(httpContext);
            if (actionAuth != null) return actionAuth.CheckLogin;
            var controllerAuth = GetAttributeOnController(httpContext);
            if (controllerAuth != null) return controllerAuth.CheckLogin;
            return false;
        }
        protected virtual string GetLoginState()
        {
            return "/Home/ShowLogin";
        }
        protected virtual string GetLoginJs()
        {
            return "window.location='/Home/Login';";
        }
        protected override ActionResult? CheckLogin(HttpContext httpContext)
        {
            IUtil? Util = httpContext.RequestServices.GetService(typeof(IUtil)) as IUtil;
            if (Util == null) { return null; }
            if (IsWinContext(httpContext))
                return null;
            var Identity = httpContext.RequestServices.GetService(typeof(IGenericIdentity<TUser>)) as IGenericIdentity<TUser>;
            if (!Identity!.HasLoginInfo())
            {
                if (Identity.GetLoginUserId() is int loginUserId && loginUserId != 0)
                {
                    var workingUserId = Identity.GetWorkingUserId();
                    var loginUser = Identity.GetLoginUser();
                    var workingUser = Identity.GetWorkingUser();
                    Identity.SetLoginInfo(new LoginInfo
                    {
                        LoginUserId = loginUserId,
                        LoginUserDeptId = GetDeptId(loginUser),
                        WorkingUserId = workingUserId,
                        WorkingUserDeptId = GetDeptId(workingUser)
                    });
                    var tokengen = httpContext.RequestServices.GetService(typeof(ILoginTokenGenerator)) as ILoginTokenGenerator;
                    tokengen!.CreateLoginToken(httpContext.Request, httpContext.Response, loginUserId);
                }
            }

            if (!Identity!.HasLoginInfo())
            {
                if (Util.IsAjaxRequest(httpContext.Request))
                    return new RichClientJsonResult(false, RichClientJsonResult.Command_Redirect, GetLoginState());
                return new ClientScriptResult(GetLoginJs());

                //for test conveniece,auto login
                //AutoLoginForTest(httpContext);

            }
            return null;
        }

        protected override bool CanAccess(HttpContext httpContext,string controllerActionName)
        {
            IUtil? Util = httpContext.RequestServices.GetService(typeof(IUtil)) as IUtil;
            var Identity = httpContext.RequestServices.GetService(typeof(IGenericIdentity<TUser>)) as IGenericIdentity<TUser>;

            if (IsWinContext(httpContext))
                return true;

            var controllerAuth = GetAttributeOnController(httpContext);
            var actionAuth = GetAttribute(httpContext);
            var funcCode = (actionAuth?.FuncCode)?? controllerAuth?.FuncCode ?? controllerActionName;

            if (!Util!.FuncMap.ContainsKey(funcCode)) return true;

            var workingUser = Identity!.RefreshUser(Identity!.GetLoginInfo()!.WorkingUserId!.Value);
            try
            {
                return workingUser.CanAccess(funcCode);
            }
            catch
            {
                workingUser = Identity.GetWorkingUser();
                if (workingUser == null)
                {
                    return false;
                }
                else
                {
                    return workingUser.CanAccess(funcCode);
                }
            }
        }

        //protected void AutoLoginForTest(HttpContext httpContext)
        //{
        //    var AdminBD = CastleContainer.WindsorContainer.Resolve<IAdminBD>();
        //    var loginUser = AdminBD.GetLoginUser("admin", AppBaseController.DefaultPassword);
        //    var loginInfo = new LoginInfoViewModel();
        //    loginInfo.LoginUser = loginUser;

        //    foreach (var func in loginUser.Funcs)
        //    {
        //        loginInfo.AddUserFuncCode(func.Code);
        //    }
        //    httpContext.Session["LoginUser"] = loginInfo;
        //}

    }
}
