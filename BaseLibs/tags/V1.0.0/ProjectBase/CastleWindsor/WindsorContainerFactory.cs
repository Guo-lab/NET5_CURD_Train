using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Application;
using System;

namespace ProjectBase.CastleWindsor
{
    public class WindsorContainerFactory : IServiceProviderFactory<IServiceCollection>
    {
        private static WindsorContainer container;
        private IServiceCollection services;
        public WindsorContainerFactory()
        {
            container = new WindsorContainer();
        }
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            this.services = services;
            return services;
        }
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)=>
            WindsorRegistrationHelper.CreateServiceProvider(container, services);
    }
}
