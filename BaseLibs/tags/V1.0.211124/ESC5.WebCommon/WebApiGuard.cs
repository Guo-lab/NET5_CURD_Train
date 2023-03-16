using ESC5.AppBase;
using ESC5.WebCommon;
using IdentityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProjectBase.BusinessDelegate;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using ProjectBase.Web.Mvc.Angular;
using SharpArch.Domain;
using System;
using System.Linq;
using System.Text;

namespace ESC5.WebCommon
{

    /**
	 * 接口访问检查：检查请求方令牌、频度等，记录日志。此处请求方对象为表单。
	 * 如果设置了一键登录且符合条件，则可以进行一键登录。
	 * @author Rainy
	 * --basic
	 * @see {@link WebApiFlag}
	 */
    public class WebApiGuard<TUser> : IAuthorizationFilter, IFormTokenGenerator, ILoginTokenGenerator, IOrderedFilter
    {

        private const string RequestAttr_WebApiLog = "WebApiLog";
        private const string COOKIENAME_LOGINTOKEN_PREFIX = "LoginToken_";
        //static private int sessionExpireDefault = 2400; //秒， 40分钟

        public WebApiGuard(IUtil util, IGenericIdentity<TUser> identity, IBaseSessionHelper sessionHelper, IDbDataEncrypter encrypter, IHostEnvironment env, IApplicationStorage storage)
        {
            Util = util;
            Identity = identity;
            SessionHelper = sessionHelper;
            Encrypter = encrypter;
            Env = env;
            AppStorage = storage;
        }

        public IUtil Util { get; set; }
        public IHostEnvironment Env { get; set; }
        public IGenericIdentity<TUser> Identity { get; set; }
        public IBaseSessionHelper SessionHelper { get; set; }
        public IDbDataEncrypter Encrypter { get; set; }
        public IApplicationStorage AppStorage { get; set; }

        public int Order => -99;

        public virtual void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            //仅请求页面直接通过
            if (!filterContext.HttpContext.Request.Query.ContainsKey(GlobalConstant.Key_For_ForViewModelOnly)) return;

            var descriptor = (ControllerActionDescriptor)filterContext.ActionDescriptor;
            string actionCode = descriptor.ControllerName + "." + descriptor.ActionName;
            ActionResult? failureResult;
            string? err = null;
            var request = filterContext.HttpContext.Request;
            int? userId = Identity.GetLoginInfo()?.LoginUserId;
            var actionMethod = descriptor.MethodInfo;
            var flag = actionMethod.GetCustomAttributes(false).OfType<WebApiFlagAttribute>().FirstOrDefault();
            if (actionMethod.GetCustomAttributes(false).OfType<HttpPostAttribute>().FirstOrDefault() != null
                && (flag == null || !flag.IgnoreFormToken))
            {
                //如果设置过formToken才检查提交的token是否相符，否则不需要检查。像用户注册那种不需要登录的提交功能也是在此处检查token。
                if (SessionHelper.HasFormToken(request))
                {
                    err = CheckFormToken(request, userId, actionCode);
                }
                else
                {
                    //如果提交了token而session中没设置过且没登录，一般是session过期了，这时提示用户刷新页面
                    string? formToken = GetFormToken(request);
                    if (formToken != null)
                    {
                        filterContext.Result = new RichClientJsonResult(false, RichClientJsonResult.Command_Message, "FormTokenExpired");
                        return;
                    }
                    //if (flag != null && flag.GuestFormTokenRequired)
                    //{
                    //    err = "NoFormTokenInSession";
                    //}
                }
            }

            WriteLog(request.HttpContext.Session.Id, userId, actionCode, err, request);
            if (err != null)
            {
                failureResult = new RichClientJsonResult(false, "InvalidRequest", null);
                filterContext.Result = failureResult;
            }
            else if (flag == null || !flag.IgnoreLoginToken)
            {//userId!=null表示找到了保持的session,即cookie与session关联上了，否则是找不到对应的session，后面需新建，这两种情况都需要先进行登录令牌检测
                failureResult = CheckLoginToken(request, filterContext.HttpContext.Response, actionCode);
                if (failureResult != null)
                {
                    filterContext.Result = failureResult;
                }
            }
        }

        //public string? CheckFrequency(HttpRequest request, int? userId, string actioncode, FrequencyAttribute frequency)
        //{
        //    string? err = null;
        //    //var ok = true;
        //    //int interval = 0;

        //    //if (frequency != null && frequency.ShouldCheck == false) return null;
        //    //if (frequency == null || frequency.Interval == 0)
        //    //{
        //    //    interval = userId == null ? DefaultNoLoginInterval : DefaultLoggedinInterval;
        //    //}
        //    //else
        //    //{
        //    //    interval = frequency.Interval;
        //    //}

        //    //Criteria apiWhere = where("WebApi").is(actioncode);

        //    ///* 同一用户或session同一api访问是否过频 */
        //    //Criteria where = apiWhere;
        //    //string sessionId=request.getSession().getId();
        //    //if (userId == null) {
        //    //	where = where.and("SessionId").is(sessionId);
        //    //} else {
        //    //	where = where.and("UserId").is(userId);
        //    //}
        //    //WebApiLog log = mongo.findOne(query(where).with(Sort.by(Direction.DESC, "ActionTime")), WebApiLog.class);
        //    //if (log !=null) {
        //    //	if (interval>0) {
        //    //		ok = log.getActionTime().isBefore(LocalDateTime.now().minus(interval,ChronoUnit.MILLIS));
        //    //		if (!ok) err = "WebApi_WaitInterval";
        //    //	}

        //    //	/* 特定api访问是否超限 */
        //    //	if (err==null && frequency != null && frequency.timesPerSession() > 0) {
        //    //		where = where.and("ActionTime").gte(LocalDateTime.now().minusDays(1));
        //    //		long times = mongo.count(query(where), WebApiLog.class);
        //    //		ok = frequency.timesPerSession() >= times;
        //    //		if (!ok) err = "WebApi_TimesPerSessionExceed";
        //    //	}
        //    //	if (err==null && frequency != null && frequency.times() > 0 && frequency.days()>0) {
        //    //		where = where.and("ActionTime").gte(LocalDateTime.now().minusDays(frequency.days()));
        //    //		long times = mongo.count(query(where), WebApiLog.class);
        //    //		ok = frequency.times() >= times;
        //    //		if (!ok) err = "WebApi_TimesInDaysExceed";
        //    //	}	
        //    //}
        //    return err;
        //}

        #region "辅助方法"
        protected virtual string? GetClientType(HttpRequest request)
        {
            var channel = AppStorage.GetAppSetting("TokenChannel").ToLower();
            if (channel == "header")
            {
                return request.Headers[AppGlobalConstant.NAME_CLIENTTYPE];
            }
            else
            {
                var method = request.Method.ToLower();
                if (method == "get" && request.Query.ContainsKey(AppGlobalConstant.NAME_CLIENTTYPE))
                {
                    return request.Query[AppGlobalConstant.NAME_CLIENTTYPE];
                }
                else if (method == "post" && request.Form.ContainsKey(AppGlobalConstant.NAME_CLIENTTYPE))
                {
                    return request.Form[AppGlobalConstant.NAME_CLIENTTYPE];
                }
                return null;
            }
        }
        protected virtual void WriteLog(string sessionId, int? userId, string actioncode, string? err, HttpRequest request)
        {
            if (Env.IsDevelopment())
            {
                Console.WriteLine("WebApiGuard:----" + actioncode + " : " + err ?? "");
            }
        }

        protected virtual ActionResult? RedirectToLogin(HttpRequest request)
        {
            if (request.Query[GlobalConstant.Key_For_ForViewModelOnly].Single() == "true")
            {
                return new RichClientJsonResult(false, RichClientJsonResult.Command_Redirect, "/Home/ShowLogin");
            }
            else
            {
                return null;
            }
        }

        [Obsolete("即将废弃，可使用Util.GetWebRoot()替代")]
        public  string GetWebRoot(HttpRequest request)
        {
            return new System.Text.StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .ToString();
        }

        public  string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        
        protected virtual bool SupportCookieChannel()
        {//根据所支持的客户端特性返回是否支持用cookie传递令牌 
            return true;
        }
        
        #endregion

        #region "Form Token"

        private string? GetFormToken(HttpRequest request)
        {
            var channel = AppStorage.GetAppSetting("TokenChannel").ToLower();
            if (channel == "header")
            {
                return request.Headers[AppGlobalConstant.NAME_FORMTOKEN];
            }
            else
            {
                return request.Form[AppGlobalConstant.NAME_FORMTOKEN];
            }
        }
        public string? CheckFormToken(HttpRequest request, int? userId, string actionCode)
        {
            string? formToken = GetFormToken(request);
            if (formToken == null) return "InvalidFormToken";
            string[] parts = formToken.Split("##");
            if (parts.Length != 2) return "InvalidFormToken";
            if (!SessionHelper.HasFormToken(request, parts[0], parts[1])) return "InvalidFormToken";
            return null;
        }
        public string CreateFormToken(HttpRequest request, Type vmClass)
        {
            string token = Util.CalcCheckCode(request.HttpContext.Session.Id, vmClass.GetHashCode(), DateTime.Now, RandomString(5));
            SessionHelper.SaveFormToken(request, vmClass.Name, token);
            return vmClass.Name + "##" + token;
        }
        #endregion

        #region"Login Token"

        protected virtual ActionResult? CheckLoginToken(HttpRequest request, HttpResponse response, string actionCode)
        {
            int? userId = Identity.GetLoginInfo()?.LoginUserId;
            string? err = "NoLoginToken";
            string? loginToken = null;
            var channel = AppStorage.GetAppSetting("TokenChannel").ToLower();
            if (!SupportCookieChannel()) {
                channel = AppStorage.GetAppSetting("NonCookieTokenChannel").ToLower();
            }
            switch (channel)
            {
                case "header":
                    if (request.Headers.ContainsKey(AppGlobalConstant.NAME_LOGINTOKEN))
                    {
                        loginToken = request.Headers[AppGlobalConstant.NAME_LOGINTOKEN];
                        err = null;
                    }
                    break;
                case "cookie":
                    var tokenCookie = request.Cookies.Where((cookie) => cookie.Key.StartsWith(COOKIENAME_LOGINTOKEN_PREFIX)).FirstOrDefault();
                    if (tokenCookie.Key != null)
                    {
                        loginToken = tokenCookie.Value;
                        err = null;
                    }
                    break;
                case "request":
                    var method = request.Method.ToLower();
                    if (method == "get" && request.Query.ContainsKey(AppGlobalConstant.NAME_LOGINTOKEN))
                    {
                        loginToken = request.Query[AppGlobalConstant.NAME_LOGINTOKEN];
                        err = null;
                    }
                    else if (method == "post" && request.Form.ContainsKey(AppGlobalConstant.NAME_LOGINTOKEN))
                    {
                        loginToken = request.Form[AppGlobalConstant.NAME_LOGINTOKEN];
                        err = null;
                    }
                    break;
            }
            if (err == null)
            {
                if (loginToken != null)
                {
                    var token = ParseLoginToken(loginToken);
                    if (token != null)
                    {
                        Identity.SetLoginToken(token);
                        if (userId == null || Identity.IsLoginTokenValid(token))
                        {//令牌正确或有令牌但session已失效，则继续到Auth处去checklogin
                            return null;
                        }
                    }
                }
                err = "InvalidLoginToken";
            }

            // 登录令牌错误，记报警，当前会话作废
            WriteLog(request.HttpContext.Session.Id, userId, actionCode, err, request);
            request.HttpContext.Session.Clear();
            return RedirectToLogin(request);
        }
        private LoginToken? ParseLoginToken(string loginToken)
        {
            LoginToken? token;
            try
            {
                token = JsonConvert.DeserializeObject<LoginToken>(loginToken);
                if (token?.LoginId == 0) return null;
            }
            catch (Exception)
            {
                return null;
            }
            return token;
        }
        /**
		 * 创建登录令牌及相关信息。仅用于创建新会话时。
		 */
        public LoginToken CreateLoginToken(HttpRequest request, HttpResponse response, int userId)
        {
            //自动登录功能，token留存到session中，设置有效期
            var loginTokenStub = new LoginToken
            {
                LoginId = userId,
                WorkingId = userId,
                Key = Guid.NewGuid().ToString()
            };
            Identity.SaveLoginTokenStub(loginTokenStub);
            string tokenKey = Encrypter.ToEncrypted(loginTokenStub.Key.ToString());
            var loginToken = new LoginToken
            {
                LoginId = userId,
                WorkingId = userId,
                Key = tokenKey
            };
            var toClient = JsonConvert.SerializeObject(loginToken);
            var channel = AppStorage.GetAppSetting("TokenChannel").ToLower();
            if (channel == "header")
            {
                response.Headers[AppGlobalConstant.NAME_LOGINTOKEN] = toClient;
            }
            else if (channel == "cookie")
            {
                //下发到客户端,关闭浏览器即失效
                string cookieNameForLogin = COOKIENAME_LOGINTOKEN_PREFIX + RandomString(10);
                var ck = cookiePath(request);
                ck.HttpOnly = true;
                response.Cookies.Append(cookieNameForLogin, toClient, ck);

                //删除上次留在客户端的登录令牌
                var lastTokenCookies = request.Cookies.Where((cookie) => cookie.Key.StartsWith(COOKIENAME_LOGINTOKEN_PREFIX));
                foreach (var lastTokenCookie in lastTokenCookies)
                {
                    if (lastTokenCookie.Key != null)
                    {
                        string lastCookieNameForLogin = lastTokenCookie.Key;
                        if (!lastCookieNameForLogin.Equals(cookieNameForLogin))
                        {
                            response.Cookies.Delete(lastCookieNameForLogin,ck);
                        }
                    }
                }
            }
            else if (channel == "request")
            {
                //暂存留待RcResult来处理
                request.HttpContext.Items[AppGlobalConstant.NAME_LOGINTOKEN] = toClient;
            }

            return loginToken;
        }
        /**
         * 删除本次会话令牌。通常应在退出会话时执行。
         */
        public void RemoveLoginToken(HttpRequest request, HttpResponse response)
        {
            int? userId = Identity.GetLoginInfo()?.LoginUserId;
            if (userId.HasValue)
            {
                Identity.RemoveLoginTokenStub(userId.Value);
            }
            var tokenCookies = request.Cookies.Where((cookie) => cookie.Key.StartsWith(COOKIENAME_LOGINTOKEN_PREFIX));
            foreach (var cookie in tokenCookies)
            {
                response.Cookies.Delete(cookie.Key, cookiePath(request));
            }
        }
        private CookieOptions cookiePath(HttpRequest request)
        {
            var ck = new CookieOptions();
            ck.Path = string.IsNullOrEmpty(request.PathBase) ? "/" : request.PathBase+ "/";
            return ck;
        }
        #endregion
    }
}

public static class RichClientJsonResultExtension
{
    public static ActionResult AttatchLoginToken(this ActionResult r, HttpRequest request)
    {
        Check.Require(r is RichClientJsonResult, "必须是RichClientJsonResult类型");
        var token = request.HttpContext.Items[AppGlobalConstant.NAME_LOGINTOKEN];
        if (token == null) return r;
        var org = (RichClientJsonResult)r;
        org.Extra = new { LoginToken = token };
        return r;
    }


}
