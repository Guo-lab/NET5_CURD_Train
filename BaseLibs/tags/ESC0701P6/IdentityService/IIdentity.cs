namespace IdentityService
{
    public interface IIdentity
    {
        #region "从Session或Token中得到当前登录用户的身份"
        int GetLoginUserId();
        //int? GetLoginUserDeptId();
        int GetWorkingUserId();
        //int? GetWorkingUserDeptId();
        int? GetActingUserId();
        #endregion

        #region "Login Token的存根"
        LoginToken? GetLoginTokenStub(int loginId);
        void SaveLoginTokenStub(LoginToken token);
        void RemoveLoginTokenStub(int loginId);
        /// <summary>
        /// 验证客户端token是否与服务器端token存根相符
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsLoginTokenValid(LoginToken token);
        #endregion

        #region "WebApiGuard从Cookie或者Header中得到客户端Token并保存在HttpContext中"
        bool IsLoginTokenOk();
        void SetLoginToken(LoginToken token);
        LoginToken? GetLoginToken();
        #endregion
    }
    public class LoginToken
    {
        //登录用户Id
        public int LoginId { get; set; }
        //当前代理的用户Id。在没有代理的情况下与LoginId相同
        public int WorkingId { get; set; }
        //在服务器端存储一个GUID，下发给客户端时需要做DES加密。
        //注意，不能使用SHA256加密，因为客户端回传后需要解密与服务器端的存根比较
        public string Key { get; set; } 
    }
}
