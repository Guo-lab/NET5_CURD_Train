namespace ESC5.Email.Message
{
    public interface IEmailEvent
    {
        void Send(EmailMessage email);
    }
}
