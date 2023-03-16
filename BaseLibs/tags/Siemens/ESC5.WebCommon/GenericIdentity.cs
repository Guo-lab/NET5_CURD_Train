using IdentityService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using ProjectBase.Domain;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using ESC5.AppBase;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ESC5.WebCommon
{
    public class GenericIdentity<TUser> : IGenericIdentity<TUser> where TUser: BaseDomainObjectWithTypedId<int>, IUserBase
    {
        private const string LOGIN_TOKEN_STUB_CACHE = "LoginTokenStubCache";
        public static readonly string CLIENT_LOGIN_TOKEN = "ClientLoginToken";
        public static readonly string LOGIN_INFO = "loginInfo";
        public IHttpContextAccessor Hca { get; set; }
        public IMemoryCache Cache { get; set; }
        public IGenericDaoWithTypedId<TUser, int> UserDao { get; set; }
        public IDbDataEncrypter Encrypter { get; set; }

        private HttpRequest Request
        {
            get
            {
                return Hca.HttpContext!.Request;
            }
        }
        private ISession Session
        {
            get
            {
                return Hca.HttpContext!.Session;
            }
        }

        public GenericIdentity(IMemoryCache cache)
        {
            Cache = cache;
            //每用户的每客户端一个loginToken
            Cache.Set(LOGIN_TOKEN_STUB_CACHE, new ConcurrentDictionary<int, Dictionary<string,LoginToken>>());
        }
        protected virtual string GetClientType()
        {
            if(Request.Method.ToLower()=="post" && Request.Form.ContainsKey(AppGlobalConstant.NAME_CLIENTTYPE)
                || Request.Query.ContainsKey(AppGlobalConstant.NAME_CLIENTTYPE)
                || Request.Headers.ContainsKey(AppGlobalConstant.NAME_CLIENTTYPE))
            {
                return "Mobile";
            }
            return "Web";
        }
        #region "从Session或Token中得到当前登录用户的身份"
        public int GetLoginUserId()
        {
            LoginInfo? loginInfo = GetLoginInfo();
            if (loginInfo != null)
            {
                int? loginUserId = loginInfo.LoginUserId;
                if (loginUserId.HasValue)
                {
                    return loginUserId.Value;
                }
            }
            var token = (LoginToken?)Hca.HttpContext!.Items[CLIENT_LOGIN_TOKEN];
            if (token == null)
            {
                return 0;
            }
            else
            {
                return token.LoginId;
            }
        }

        //public int? GetLoginUserDeptId()
        //{
        //    return GetLoginInfo()!.LoginUserDeptId;
        //}

        public int GetWorkingUserId()
        {
            LoginInfo? loginInfo = GetLoginInfo();
            if (loginInfo != null)
            {
                int? workingUserId = loginInfo.WorkingUserId;
                if (workingUserId.HasValue)
                {
                    return workingUserId.Value;
                }
            }

            var token = (LoginToken?)Hca.HttpContext!.Items[CLIENT_LOGIN_TOKEN];
            if (token == null)
            {
                return 0;
            }
            else
            {
                return token.WorkingId;
            }
        }

        //public int? GetWorkingUserDeptId()
        //{
        //    return GetLoginInfo()!.WorkingUserDeptId;
        //}

        public int? GetActingUserId()
        {
            int loginUserId = GetLoginUserId();
            int workingUserId = GetWorkingUserId();
            if (loginUserId == workingUserId)
            {
                return null;
            }
            else
            {
                return loginUserId;
            }
        }
        #endregion

        #region "IGenericIdentity接口的实现，根据项目需要得到具体的Domain对象"
        public TUser? GetLoginUser(string userCode, string password)
        {
            TUser user = UserDao.GetOneByQuery(o => o.Code == userCode);
            if (user != null && user.IsActive)
            {
                if (SHA256.AreEqual(password, user.Password, user.Salt))
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public TUser RefreshUser(int userId)
        {
            TUser user = UserDao.Get(userId);
            UserDao.Refresh(user);
            return user;
        }

        //-----
        /// <summary>
        /// 注意，调用此方法时如果需要用到User对象的Dept,Supervisor等many-to-one属性或者Funcs，Commodities等one-to-many
        /// 属性，务必传递refresh=true再使用
        /// 
        /// </summary>
        /// <returns></returns>
        public TUser? GetWorkingUser(bool refresh = true)
        {
            int? userId = GetWorkingUserId();
            if (userId == null || userId == 0) { return null; }

            TUser workingUser = UserDao.Get(userId.Value);

            if (workingUser == null)
                return null;
            else
            {
                if (refresh) UserDao.Refresh(workingUser);
                return workingUser;
            }
        }
        public TUser? GetLoginUser(bool refresh = true)
        {
            int? userId = GetLoginUserId();
            if (userId == null || userId == 0) { return null; }

            TUser loginUser = UserDao.Get(userId.Value);

            if (loginUser == null)
                return null;
            else
            {
                if (refresh) UserDao.Refresh(loginUser);
                return loginUser;
            }
        }

        public LoginInfo? GetLoginInfo()
        {
            return Session.Get<LoginInfo>(LOGIN_INFO);
        }
        /**
		 * 在session中保存LoginInfo.
		 */
        public void SetLoginInfo(LoginInfo loginInfo)
        {
            Session.Set(LOGIN_INFO, loginInfo);
        }
        public bool HasLoginInfo()
        {
            return Session.ContainsKey(LOGIN_INFO);
        }
        #endregion

        #region "Login Token存根"
        private ConcurrentDictionary<int, Dictionary<string, LoginToken>> GetStore()
        {
            return Cache.Get<ConcurrentDictionary<int, Dictionary<string, LoginToken>>>(LOGIN_TOKEN_STUB_CACHE);
        }
        private Dictionary<string, LoginToken>? GetTokenMap(int loginId)
        {
            if (GetStore().TryGetValue(loginId, out Dictionary<string, LoginToken>? tokens))
            {
                return tokens;
            }
            return null;
        }

        public LoginToken? GetLoginTokenStub(int loginId)
        {
            if (GetStore().TryGetValue(loginId, out Dictionary<string, LoginToken>? tokens))
            {
                if(tokens.TryGetValue(GetClientType(), out LoginToken? token)) 
                {
                    return token;
                }
            }
            return null;
        }

        public void SaveLoginTokenStub(LoginToken token)
        {
            GetStore().AddOrUpdate(token.LoginId,
                (loginId) =>
                {
                    var tokenMap = new Dictionary<string, LoginToken>();
                    tokenMap.Add(GetClientType(), token);
                    return tokenMap;
                },
                (loginId, existingTokenMap) =>
                {
                    existingTokenMap[GetClientType()] = token;
                    return existingTokenMap;
                    //existingToken.WorkingId = token.WorkingId;
                    //existingToken.Key = token.Key;
                    //return existingToken;
                });
        }

        public void RemoveLoginTokenStub(int loginId)
        {
            var tokenMap = GetTokenMap(loginId);
            if (tokenMap == null) return;
            tokenMap.Remove(GetClientType());
            if (tokenMap.Count == 0)
            {
                GetStore().Remove(loginId,out Dictionary<string,LoginToken>? removedTokens);
            }
        }

        public bool IsLoginTokenValid(LoginToken token)
        {
            var stub = GetLoginTokenStub(token.LoginId);
            if (stub == null) return false;
            var submittedKey = Encrypter.ToDecrypted(token.Key);
            return submittedKey.Equals(stub.Key);
        }
        #endregion

        #region "WebApiGuard从Cookie或者Header中得到客户端Token，保存在HttpContext.Item中"
        public void SetLoginToken(LoginToken token)
        {
            Hca.HttpContext!.Items[CLIENT_LOGIN_TOKEN] = token;
        }
        public LoginToken? GetLoginToken()
        {
            return (LoginToken?)Hca.HttpContext!.Items[CLIENT_LOGIN_TOKEN];
        }

        public bool IsLoginTokenOk()
        {
            return GetLoginToken() != null;
        }
        #endregion
    }
}
