using ProjectBase.BusinessDelegate;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESC5.Email.Message
{
    public class EmailBodyGenerator: IEmailBodyGenerator
    {
        public IApplicationStorage ApplicationStorage { get; set; }

        private Dictionary<string, string> Templates;
        public string GenerateBody(string template, Dictionary<string,string> values, Dictionary<string,IList<Dictionary<string,string>>> loopValues)
        {
            string emailTemplate = GetTemplate(template);
            foreach (KeyValuePair<string,string> kvp in values)
            {
                emailTemplate = emailTemplate.Replace("<" + kvp.Key + ">", kvp.Value);
            }
            if (loopValues != null)
            {
                foreach(var kvp in loopValues)
                {
                    int start = emailTemplate.IndexOf("@" + kvp.Key + "@") + kvp.Key.Length + 2;
                    int end = emailTemplate.IndexOf("@/" + kvp.Key + "@");
                    //把循环体内的模板保存下来
                    string loopTemplate = emailTemplate.Substring(start, end - start);
                    //从Body内去除循环模板
                    emailTemplate = emailTemplate.Substring(0, start) + emailTemplate.Substring(end);

                    StringBuilder loopResult = new StringBuilder("");
                    foreach (var loopValue in kvp.Value)
                    {
                        string translated = loopTemplate;
                        foreach (var kvpLoop in loopValue)
                        {
                            translated = translated.Replace("<" + kvpLoop.Key + ">", kvpLoop.Value);
                        }
                        loopResult.Append(translated);
                    }
                    emailTemplate = emailTemplate.Insert(start, loopResult.ToString());
                    emailTemplate = emailTemplate.Replace("@" + kvp.Key + "@", "");
                    emailTemplate = emailTemplate.Replace("@/" + kvp.Key + "@", "");
                }
            }
            return emailTemplate;
        }

        public string GenerateBody(string template, Dictionary<string, string> values)
        {
            return GenerateBody(template, values, null);
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
