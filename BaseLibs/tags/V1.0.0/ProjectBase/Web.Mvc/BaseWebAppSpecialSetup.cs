using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProjectBase.Application;
using ProjectBase.Web.Mvc.Angular;
using ProjectBase.Web.Mvc.Tool;
using ProjectBase.Web.Mvc.Validation;
using SharpArch.NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectBase.Web.Mvc
{
    public class BaseWebAppSpecialSetup : IAppSpecialSetup
    {
        public IHostEnvironment Env { get; set; }
        public IWindsorContainer WindsorContainer { get; set; }
        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigViewEngine(services);
            ConfigMvc(services);
            ConfigSession(services);
            ConfigDevTool(services);
        }

        public virtual void SetupSpecialFeature()
        {
            RegisterComponents();
            //var pathBase = Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");
            //UrlHelperExtension.ApplicationPath = pathBase;
        }

        protected virtual void RegisterComponents()
        {
            WindsorContainer.Register(
                Component.For(typeof(ISessionStorage))
                    .ImplementedBy(typeof(WebHibernateSessionTaskStorage))
                    .LifestyleSingleton());
        }
        private void ConfigViewEngine(IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("~/{1}/{0}.cshtml");
                o.ViewLocationFormats.Add("~/Shared/{0}.cshtml");
                o.AreaViewLocationFormats.Add("~/{2}/{1}/{0}.cshtml");
                o.AreaViewLocationFormats.Add("~/{2}/Shared/{0}.cshtml");
            });
        }
        protected virtual void ConfigMvc(IServiceCollection services)
        {
            var corsOrigins = Configuration["CORS:AllowOrigins"];
            var allowHeaders = Configuration["CORS:AllowHeaders"];
            var exposedHeaders = Configuration["CORS:ExposedHeaders"];
            if (corsOrigins != null)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("CustomCorsPolicy",
                        builder =>
                        {
                            builder.WithOrigins(corsOrigins.Split(','))
                                    .WithHeaders(allowHeaders.Split(','))
                                    .AllowAnyMethod()
                                    .AllowCredentials()
                                    .WithExposedHeaders(exposedHeaders.Split(','));
                        });
                });
            }
            services.AddControllersWithViews()
                .AddControllersAsServices()
                .AddJsonOptions((o) =>
                {
                    o.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            services.AddHttpContextAccessor();
            //services.AddSingleton<AppBaseControllerExceptionFilter>();应用层应加这句
            services.Configure<MvcOptions>((o) =>
            {
                o.Filters.Add(typeof(ValidateFilter));
                //o.Filters.Add(typeof(AppBaseControllerExceptionFilter));应用层应加这句
                //o.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider());
            });
            services.AddSingleton<IModelMetadataProvider, CustomModelMetadataProvider>();
            services.Configure<MvcViewOptions>((o) =>
            {
                o.ClientModelValidatorProviders.RemoveAt(2);
                o.ClientModelValidatorProviders.Add(new ClientDataTypeModelValidatorProvider());

            });
            services.AddMemoryCache();
        }
        private void ConfigDevTool(IServiceCollection services)
        {
            try
            {
                var filepath = (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\toolsettings.json";
                dynamic obj = JsonConvert.DeserializeObject(File.ReadAllText(filepath));

                var path = (string)obj.DevTool["VmMetaOutputFolderRoot"];
                var includes = (string)obj.DevTool["VmMetaIncludesModules"];
                if (path != null)
                {
                    ClientVmMetadataProvider.Config(path);
                    ClientVmDefinitionTool.Config(path, includes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("开发工具运行错：");
                Console.Write(e.StackTrace);
            }
        }
        private void ConfigSession(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(double.Parse(Configuration["SessionTimeout"]) * 60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

    }
}