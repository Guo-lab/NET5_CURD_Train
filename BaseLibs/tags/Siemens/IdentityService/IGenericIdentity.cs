namespace IdentityService
{
    public interface IGenericIdentity<TUser> : IIdentity 
    {
        TUser? GetLoginUser(string userCode, string password);
        TUser RefreshUser(int userId);
        TUser? GetWorkingUser(bool refresh = false);
        TUser? GetLoginUser(bool refresh = false);
        LoginInfo? GetLoginInfo();
        bool HasLoginInfo();
        void SetLoginInfo(LoginInfo loginInfo);
    }
    /// <summary>
    /// @author Rainy
    /// </summary>
    public class LoginInfo
    {
        public int LoginUserId { get; set; }
        public int? LoginUserDeptId { get; set; }
        public int? WorkingUserId { get; set; }
        public int? WorkingUserDeptId { get; set; }
    }
}
