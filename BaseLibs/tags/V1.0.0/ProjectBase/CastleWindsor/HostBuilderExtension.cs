using Microsoft.Extensions.Hosting;

namespace ProjectBase.CastleWindsor
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder UseWindsorContainer(this IHostBuilder hostBuilder) =>
            hostBuilder.UseServiceProviderFactory(new WindsorContainerFactory());
    }
}

