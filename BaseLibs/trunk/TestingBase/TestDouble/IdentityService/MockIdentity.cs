using IdentityService;

namespace TestingBase.TestDouble.IdentityService
{
    public class MockIdentity : IIdentity
    {
        public int? GetActingUserId()
        {
            return MockIdentityValue.ActingUserId;
        }

        public LoginToken GetLoginToken()
        {
            throw new System.NotImplementedException();
        }

        public LoginToken GetLoginTokenStub(int userId)
        {
            throw new System.NotImplementedException();
        }

        public int? GetLoginUserDeptId()
        {
            return MockIdentityValue.LoginUserDeptId;
        }
        
        public int GetLoginUserId()
        {
            return MockIdentityValue.LoginUserId;
        }
        
        public int? GetWorkingUserDeptId()
        {
            return MockIdentityValue.WorkingUserDeptId;
        }
      
        public int GetWorkingUserId()
        {
            return MockIdentityValue.WorkingUserId;
        }

        public bool IsLoginTokenValid(LoginToken token)
        {
            throw new System.NotImplementedException();
        }

        public bool IsLoginTokenOk()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveLoginTokenStub(int userId)
        {
            throw new System.NotImplementedException();
        }

        public void SaveLoginTokenStub(LoginToken token)
        {
            throw new System.NotImplementedException();
        }

        public void SetLoginToken(LoginToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}
