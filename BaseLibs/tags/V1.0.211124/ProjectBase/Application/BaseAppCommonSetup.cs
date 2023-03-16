using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectBase.AutoMapper;
using ProjectBase.Data;
using ProjectBase.Utils;
using SharpArch.NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ProjectBase.Application
{
    public abstract class BaseAppCommonSetup : IAppCommonSetup
    {
        public IHostEnvironment Env { get; set; }
        public IWindsorContainer WindsorContainer { get; set; }
        public IConfiguration Configuration { get; set; }
        public BaseAppCommonSetup()
        {
            InitProjectHierarchy();
        }
        protected abstract void InitProjectHierarchy();

        public void ConfigureServices(IServiceCollection services) {
            InitAutoMapper(services);
        }

        protected abstract IDictionary<string, string> GetNamespaceMapToTablePrefix();

        /// <summary>
        /// 注册本项目特有的接口和实现类的对应关系
        /// </summary>
        protected abstract void CustomContainerRegister();

        public void SetupCommonFeature()
        {
            InitLog4net();
            AddComponentsTo();
            CustomContainerRegister();

            NHibernateSession.RegisterInterceptor(new MyInterceptor());
            NHibernateSessionModified.AfterInit = AfterNHInit;
        }

        protected virtual void AfterNHInit()
        {
        }

        protected virtual void InitLog4net()
        {
            XmlConfigurator.Configure(new FileInfo(GetRunningPath("Log4net.config")));
        }
        protected virtual void AddComponentsTo()
        {
            CastleWindorComponentRegistrar.AddComponentsTo(WindsorContainer);
        }

        protected virtual void InitAutoMapper(IServiceCollection services)
        {
            AutoMapperProfile.DomainModelMappingAssemblies = GetDomainModelMappingAssemblies();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapperProfile)));
        }
        protected virtual string[] GetDomainModelMappingAssemblies()
        {
            string[] list = ProjectHierarchy.DomainNS.Split(',');
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = GetRunningPath(list[i] + ".dll");
            }
            return list;
        }

        public string GetRunningPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\" + relativePath;
        }

        public virtual NHibernate.Cfg.Configuration InitializeNHibnerate(SharpArch.NHibernate.ISessionStorage storage)
        {
            WindsorContainer.Register(
                Component.For(typeof(ISessionFactoryKeyProvider))
                    .ImplementedBy(typeof(DefaultSessionFactoryKeyProvider))
                    .Named("sessionFactoryKeyProvider"));

            //NHibernateSession.ConfigurationCache = new NHibernateConfigurationFileCache();
            NHibernateSessionModified.NamespaceMapToTablePrefix = GetNamespaceMapToTablePrefix();
            //NHibernateSessionModified.OutputXmlMappingsFile = GetRunningPath("NHibernate.mapping.xml");
            return NHibernateSessionModified.Init(
                storage,
                GetDomainModelMappingAssemblies(),
                GetRunningPath("NHibernate.config"),
                new Dictionary<string, string>(),
                null
            );
        }
    }
}
