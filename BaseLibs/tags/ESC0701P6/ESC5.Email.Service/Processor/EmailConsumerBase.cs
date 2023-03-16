using ESC5.Email.Message;
namespace ESC5.Email.Service.Processor
{
    public class EmailConsumerBase
    {
        private IEmailProcessor processor;
        protected IEmailProcessor EmailProcessor
        {
            get
            {
                if (processor == null)
                {
                    processor = EmailProcessorFactory.CreateEmailProcessor();
                }
                return processor;
            }

        }
    }
}
