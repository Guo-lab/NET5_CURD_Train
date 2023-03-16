using ProjectBase.BusinessDelegate;
using System.Collections.Generic;
using System.IO;

namespace ESC5.Email.Message
{
    public class EmailBodyGenerator: IEmailBodyGenerator
    {
        public IApplicationStorage ApplicationStorage { get; set; }

        private Dictionary<string, string> Templates;
        public string GenerateBody(string template, Dictionary<string,string> values)
        {
            string emailTemplate = GetTemplate(template);
            foreach (KeyValuePair<string,string> kvp in values)
            {
                emailTemplate = emailTemplate.Replace("<" + kvp.Key + ">", kvp.Value);
            }
            return emailTemplate;
        }

        private string GetTemplate(string template)
        {
            if (Templates == null)
                Templates = new Dictionary<string, string>();
            if (!Templates.ContainsKey(template))
            {
                string body;
                lock (Templates)
                {
                    
                    StreamReader oReader = new StreamReader(GetEmailTemplatePath() + "\\" + template, System.Text.Encoding.UTF8);
                    body = oReader.ReadToEnd();
                    oReader.Close();
                }
                Templates.Add(template, body);
                return body;
            }
            else
            {
                return Templates[template];
            }
        }
        private string GetEmailTemplatePath()
        {
            string templatePath = ApplicationStorage.GetAppSetting("EmailTemplatePath");
            if (string.IsNullOrEmpty(templatePath))
            {
                templatePath = ApplicationStorage.GetRealPath("EmailTemplate");
            }
            return templatePath;
        }
    }
}
