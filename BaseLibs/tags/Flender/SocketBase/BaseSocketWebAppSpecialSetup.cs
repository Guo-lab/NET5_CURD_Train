using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectBase.Application;
using SharpArch.NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace SocketBase
{
    public class BaseSocketWebAppSpecialSetup : IAppSpecialSetup
    {
        public IHostEnvironment Env { get; set; }
        public IWindsorContainer WindsorContainer { get; set; }
        public IConfiguration Configuration { get; set; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigSignalRMC(services);
        }

        public virtual void SetupSpecialFeature()
        {
            RegisterComponents();
            AddControllersAsServices();
        }

        private void AddControllersAsServices()
        {
            //AddControllersAsServices
            AssemblyFilter allAssembly = new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory);
            WindsorContainer.Register(
                Classes.FromAssemblyInDirectory(allAssembly)
                    .BasedOn<IController>()
                    .WithServices(typeof(IController))
                    .LifestyleTransient()
                    .Configure(o=> {
                        var areaAttr = o.Implementation.CustomAttributes.OfType<AreaAttribute>().FirstOrDefault();
                        var area = areaAttr?.Value ?? FrontController.DEFAULT_AREA;
                        o.Named(area+"."+ o.Implementation.Name);
                    })
            );
        }
        protected virtual void RegisterComponents()
        {
            WindsorContainer.Register(
                Component.For(typeof(ISessionStorage))
                    .ImplementedBy(typeof(SocketHibernateSessionTaskStorage))
                    .LifestyleSingleton());
        }
       
        protected virtual void ConfigSignalRMC(IServiceCollection services)
        {
            services.AddOptions();
            var corsOrigins = Configuration["CORS:AllowOrigins"];
            var allowHeaders = Configuration["CORS:AllowHeaders"];
            var exposedHeaders = Configuration["CORS:ExposedHeaders"];
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(corsOrigins.Split(','))
                        .WithHeaders(allowHeaders.Split(','))
                        .AllowAnyMethod()
                        .AllowCredentials();
                    if (exposedHeaders != null)
                    {
                        builder.WithExposedHeaders(exposedHeaders.Split(','));
                    }
                });
            });

            // AddControllersAsServices();由于此处不是mvc框架注册controller，而需要用winsdor注册，因此改到其它位置执行
            services.AddSignalR()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    var resolver = new DefaultContractResolver();
                    //保证PascalCase，效果同
                    //.AddJsonProtocol(jsonOptions=> {
                    //    jsonOptions.PayloadSerializerOptions.PropertyNamingPolicy = null;
                    //});
                    options.PayloadSerializerSettings.ContractResolver = resolver;
                    options.PayloadSerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                    options.PayloadSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                });
            services.AddSingleton<ISocketContextAccessor,SocketContextAccessor>();
            services.AddMemoryCache();
        }
    }
}

