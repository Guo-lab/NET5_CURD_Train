namespace ESC5.AppBase
{
    public interface IUserBase
    {
        string Code { get; }
        bool IsActive { get; }
        string Password { get; }
        string Salt { get; }
    }
}
