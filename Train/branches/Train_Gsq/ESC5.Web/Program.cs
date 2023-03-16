using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ProjectBase.CastleWindsor;

namespace ESC5.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindsorContainer()
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder.UseWebRoot("./");
                  })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    
                });
    }
}