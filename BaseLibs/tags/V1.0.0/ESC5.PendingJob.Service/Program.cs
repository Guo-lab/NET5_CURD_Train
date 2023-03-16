using ESC5.PendingJob.Service.Handler;
using log4net;
using log4net.Config;
using MassTransit.Log4NetIntegration.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using Topshelf;
using Topshelf.Logging;
namespace ESC5.PendingJob.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLog4Net();
            // Topshelf to use Log4Net
            Log4NetLogWriterFactory.Use();

            // MassTransit to use Log4Net
            Log4NetLogger.Use();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            AppSetting.WebUrl = configuration["WebUrl"];
            AppSetting.JobRemotingPort = Convert.ToInt32(configuration["JobRemotingPort"]);

            HostFactory.Run(x =>
            {
                x.Service<PendingJobServiceRemoting>(s =>
                {
                    s.ConstructUsing(name => new PendingJobServiceRemoting());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.StartAutomatically();
                x.RunAsLocalSystem();
                x.SetDescription("eSupplyChain Pending Job Service");
                x.SetDisplayName("eSupplyChain Pending Job Service");
                x.SetServiceName("ESCPendingJobService");
            });
        }

        static private void ConfigureLog4Net()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
