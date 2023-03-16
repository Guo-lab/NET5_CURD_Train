using System;
using System.Threading.Tasks;
using System.IO;
using RemotingConnector;
using ESC5.Email.Message;
using Newtonsoft.Json;

namespace ESC5.Email.Service.Processor
{
    public class EmailConsumerRemoting:EmailConsumerBase
    {
        public void Consume(EmailMessage email)
        {
            EmailProcessor.Send(email);
        }
        
    }
}
