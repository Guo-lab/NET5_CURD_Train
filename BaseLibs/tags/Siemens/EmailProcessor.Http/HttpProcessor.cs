using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESC5.Email.Message;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Configuration;
using ProjectBase.BusinessDelegate;

namespace EmailProcessor
{
    public class HttpProcessor : IEmailProcessor
    {

        HttpSetting settings;
        public IApplicationStorage  AppStorage { get; set; }
        public HttpProcessor(string configPath)
        {
            using (StreamReader reader = new StreamReader(configPath))
            {
                string setting = reader.ReadToEnd();
                reader.Close();
                settings = JsonConvert.DeserializeObject<HttpSetting>(setting);
            }
        }
        public void Send(EmailMessage message)
        {
            var env = AppStorage.GetAppSetting("Environment");
            if (env  != null && env.ToLower() == "test") {
                message.To = AppStorage.GetAppSetting("EmailReplyTo");
            }
            XmlDocument xmlDocument = GetEmailXML(message);
             HttpLibrary.HttpTransfer.Open(
                    settings.E2bizNotificationUrl,
                    System.Web.HttpUtility.UrlEncode(CreateE2bizNotificationFormat(xmlDocument.InnerXml, settings.E2bizNotificationRoot)))
                ;
        }

        private string CreateE2bizNotificationFormat(string content, string root)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
            XmlElement fileNode = doc.CreateElement("File");
            fileNode.SetAttribute("FileName", AppStorage.GetAppSetting ("CustomerCode") +"."+ Guid.NewGuid().ToString() + ".xml");
            fileNode.SetAttribute("FileType", root);
            fileNode.SetAttribute("FileSize", bytes.Length.ToString());

            XmlNode contentNode = doc.CreateElement("Content");

            contentNode.InnerText = Convert.ToBase64String(bytes);
            fileNode.AppendChild(contentNode);

            doc.AppendChild(fileNode);
            return doc.InnerXml;
        }

        private XmlDocument GetEmailXML(EmailMessage message)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
            XmlElement root = default(XmlElement);
            root = doc.CreateElement("Customer");
            if (string.IsNullOrEmpty(message.CustomerCode))
            {
                root.SetAttribute("E2bizCode", AppStorage.GetAppSetting ("CustomerCode"));
            }
            else
            {
                root.SetAttribute("E2bizCode", message.CustomerCode);
            }
            XmlElement item = GetEmailElement(doc, message);
            root.AppendChild(item);
            doc.AppendChild(root);
            return doc;
        }

        private XmlElement GetEmailElement(XmlDocument doc, EmailMessage message)
        {
            XmlElement item = doc.CreateElement("Item");
            if (!string.IsNullOrEmpty(message.ReceiverE2bizCode))
            { //只有SendPassword才需要这个属性
                item.SetAttribute("VendorCode", message.ReceiverE2bizCode);
            }
            item.SetAttribute("Type", "Email");
            XmlElement receiver = doc.CreateElement("Receiver");
            receiver.InnerText = message.To;
            item.AppendChild(receiver);

            if (!string.IsNullOrEmpty(message.CC))
            {
                XmlElement cc = doc.CreateElement("CC");
                cc.InnerText = message.CC;
                item.AppendChild(cc);
            }
            if (!string.IsNullOrEmpty(message.BCC))
            {
                XmlElement bcc = doc.CreateElement("BCC");
                bcc.InnerText = message.BCC;
                item.AppendChild(bcc);
            }
            if (!string.IsNullOrEmpty(message.ReplyTo))
            {
                XmlElement replyTo = doc.CreateElement("ReplyTo");
                replyTo.InnerText = message.ReplyTo;
                item.AppendChild(replyTo);
            }

            XmlElement format = doc.CreateElement("Format");
            format.InnerText = message.Format;
            item.AppendChild(format);

            XmlElement priority = doc.CreateElement("Priority");
            switch (message.Priority)
            {
                case "Normal":
                    priority.InnerText = "0";
                    break;
                case "Low":
                    priority.InnerText = "1";
                    break;
                case "High":
                    priority.InnerText = "2";
                    break;
                default:
                    priority.InnerText = "0";
                    break;
            }
            item.AppendChild(priority);

            XmlElement subject = doc.CreateElement("Subject");
            subject.InnerText = message.Subject;
            item.AppendChild(subject);

            XmlElement body = doc.CreateElement("Body");

            body.InnerText = XMLEncode(message.Body);

            item.AppendChild(body);

            if (message.Attachment != null && message.Attachment.Count > 0)
            {
                XmlElement isGenerateZip = doc.CreateElement("IsGenerateZip");
                isGenerateZip.InnerText = message.IsGenerateZip;
                item.AppendChild(isGenerateZip);

                XmlElement attachments = doc.CreateElement("Attachments");
                for (int i = 0; i < message.Attachment.Count; i++)
                {
                    string attachment = message.Attachment[i];
                    if (attachment != "" && File.Exists(attachment))
                    {
                        XmlElement xeAttachment = doc.CreateElement("Attachment");
                        xeAttachment.SetAttribute("FileName", Path.GetFileName(attachment));
                        xeAttachment.SetAttribute("FileSize", GetFileSize(attachment).ToString());
                        xeAttachment.InnerText = HttpLibrary.File2Base64.ConvertToBase64(attachment);
                        attachments.AppendChild(xeAttachment);
                    }
                }

                item.AppendChild(attachments);

            }
            return item;
        }

        private long GetFileSize(string fileName)
        {
            FileInfo oFileInfo = new FileInfo(fileName);
            return oFileInfo.Length;
        }
        private string XMLEncode(string sourceXML)
        {
            if (sourceXML == null)
            { return ""; }
            sourceXML = sourceXML.Replace("&", "&amp;");
            sourceXML = sourceXML.Replace("<", "&lt;");
            sourceXML = sourceXML.Replace(">", "&gt;");
            sourceXML = sourceXML.Replace("\"", "&quot;");
            sourceXML = sourceXML.Replace("'", "&apos;");
            sourceXML = sourceXML.Replace("\r\n", "{CRLF}");

            return sourceXML;
        }


    }
}
