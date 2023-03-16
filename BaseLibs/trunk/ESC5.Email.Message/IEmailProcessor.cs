namespace ESC5.Email.Message
{
    public interface IEmailProcessor
    {
        void Send(EmailMessage message);
    }
}
