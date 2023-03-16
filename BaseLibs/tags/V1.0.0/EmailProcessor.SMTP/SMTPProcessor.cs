using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using ESC5.Email.Message;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace EmailProcessor
{
   
    public class SMTPProcessor: IEmailProcessor
    {

        private IList<SMTPSetting> settings;
        public SMTPProcessor(string configPath)
        {
            using (StreamReader reader = new StreamReader(configPath))
            {
                string setting = reader.ReadToEnd();
                reader.Close();
                settings = JsonConvert.DeserializeObject<IList<SMTPSetting>>(setting);
            }
        }

        public void Send(EmailMessage message)
        {
            string customerCode = message.CustomerCode ??"";
            SMTPSetting setting = settings.Single(x => x.CustomerCode == customerCode);
            MailMessage emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(setting.Sender);
            foreach (string email in message.To.Replace(",", ";").Split(';'))
            {
                emailMessage.To.Add(new MailAddress(email));
            }
            if (!string.IsNullOrEmpty (message.CC))
            {
                foreach (string email in message.CC.Replace(",", ";").Split(';'))
                {
                    emailMessage.CC.Add(new MailAddress(email));
                }
            }
            if (!string.IsNullOrEmpty(message.BCC))
            {
                foreach (string email in message.BCC.Replace(",", ";").Split(';'))
                {
                    emailMessage.Bcc.Add(new MailAddress(email));
                }
            }
            if (!string.IsNullOrEmpty(message.ReplyTo))
            {
                foreach (string email in message.ReplyTo.Replace(",", ";").Split(';'))
                {
                    emailMessage.ReplyToList.Add(new MailAddress(email));
                }
            }
            emailMessage.Subject = message.Subject;
            emailMessage.Body = message.Body;
            emailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            emailMessage.IsBodyHtml = (message.Format == "HTML" ? true : false);
            switch (message.Priority)
            {
                case "Normal":
                    emailMessage.Priority = MailPriority.Normal;
                    break;
                case "High":
                    emailMessage.Priority = MailPriority.High;
                    break;
                case "Low":
                    emailMessage.Priority = MailPriority.Low ;
                    break;
                default:
                    emailMessage.Priority = MailPriority.Normal;
                    break;
            }

            SmtpClient smtpClient = new SmtpClient();
            if (setting.SMTPUID != "")
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(setting.SMTPUID , setting.SMTPPWD);
            }
           
            if (message.Attachment != null) { 
                foreach(string attachment in message.Attachment )
                    emailMessage.Attachments.Add(new Attachment(attachment));
            }

            smtpClient.Host = setting.SMTPServer;
            try
            {
                smtpClient.Send(emailMessage);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (emailMessage.Attachments.Count > 0)
                {
                    foreach (Attachment attachment in emailMessage.Attachments)
                    {
                        attachment.ContentStream.Close();
                    }
                }
            }
        }
    }
}
