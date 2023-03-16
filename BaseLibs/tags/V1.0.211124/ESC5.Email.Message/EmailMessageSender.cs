using Microsoft.Extensions.Configuration;
using ProjectBase.MessageSender;
using ProjectBase.MetaData;
using RemotingConnector;
using System;

namespace ESC5.Email.Message
{
    public class EmailMessageSender:MessageSenderBase<IEmailEvent>,IMessageSender<EmailMessage>
    {
        
        public EmailMessageSender(IConfiguration config):base(config["AppSetting:RemotingServer"], Convert.ToInt32(config["AppSetting:EmailRemotingPort"]))
        {
        }

        public void Send(EmailMessage message)
        {
            try
            {
                ConnectToServer();
                this.Proxy.Send(message);
            }
            catch (Exception ex)
            {
                Logger.Log.Info("Unable send email to service. Type:" + message.Subject + ", Receiver:" + message.To + "." + ex.Message);
            }
        }

    }
}
