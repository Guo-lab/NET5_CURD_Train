using log4net;
using log4net.Config;
using MassTransit.Log4NetIntegration.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using Topshelf.Logging;

namespace ESC5.OfflineService
{
    public abstract class OfflineServiceStartup
    {
        protected virtual void Init()
        {
            ConfigureLog4Net();
            // Topshelf to use Log4Net
            Log4NetLogWriterFactory.Use();

            // MassTransit to use Log4Net
            Log4NetLogger.Use();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            InitAppSetting(configuration);
        }

        private void ConfigureLog4Net()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        protected abstract void InitAppSetting(IConfiguration configuration);
    }
}
