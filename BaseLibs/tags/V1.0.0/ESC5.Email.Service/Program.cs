using System;
using ESC5.Email.Service.Processor;
using log4net;
using log4net.Config;
using MassTransit.Log4NetIntegration.Logging;
using Topshelf;
using Topshelf.Logging;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ESC5.Email.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigureLog4Net();
            // Topshelf to use Log4Net
            Log4NetLogWriterFactory.Use();

            // MassTransit to use Log4Net
            Log4NetLogger.Use();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            AppSetting.CustomerCode = configuration["CustomerCode"];
            AppSetting.EmailChannel = configuration["EmailChannel"];
            AppSetting.EmailRemotingPort = Convert.ToInt32(configuration["EmailRemotingPort"]);

            HostFactory.Run(x =>
            {
                x.Service<EmailServiceRemoting>(s =>
                {
                    s.ConstructUsing(name => new EmailServiceRemoting());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.StartAutomatically();
                x.RunAsLocalSystem();
                x.SetDescription("eSupplyChain Email Service");
                x.SetDisplayName("eSupplyChain Email Service");
                x.SetServiceName("ESCEmailService");
            });
        }

        static private void ConfigureLog4Net()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
