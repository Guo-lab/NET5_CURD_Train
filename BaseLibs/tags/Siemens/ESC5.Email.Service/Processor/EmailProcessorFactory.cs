using ESC5.Email.Message;
using System;

namespace ESC5.Email.Service.Processor
{
    public class EmailProcessorFactory
    {
        public static IEmailProcessor CreateEmailProcessor()
        {
            string[] channel = AppSetting.EmailChannel.Split(',');
            string type = channel[0];
            string assembly = channel[1];
            return (IEmailProcessor)Activator.CreateInstance(Type.GetType("EmailProcessor." +  type + "Processor," + assembly),new object[] { AppDomain.CurrentDomain.BaseDirectory + @"\Config\" + type + ".json" });
        }
    }
}
