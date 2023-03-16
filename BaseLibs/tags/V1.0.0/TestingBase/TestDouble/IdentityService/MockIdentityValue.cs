
namespace TestingBase.TestDouble.IdentityService
{
    public class MockIdentityValue
    {
        public static int? ActingUserId { get; set; }
        public static int LoginUserDeptId { get; set; }
        public static int LoginUserId { get; set; }
        public static int WorkingUserDeptId { get; set; }
        public static int WorkingUserId { get; set; }

        public static void Reset()
        {
            ActingUserId = null;
            LoginUserDeptId = 0;
            LoginUserId = 0;
            WorkingUserDeptId = 0;
            WorkingUserId = 0;
        }
    }
}
