using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;
using NHibernate.Tool.hbm2ddl;
using ProjectBase.Application;
using ProjectBase.BusinessDelegate;
using ProjectBase.Data;
using ProjectBase.Domain;
using ProjectBase.Domain.Transaction;
using ProjectBase.Utils;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using TestingBase.TestDouble.ProjectBase;

namespace TestingBase.TestBase
{
    /**
	 * BD测试运行的Context，即Ioc容器中的对象注册情况。
	 * 使用一个父容器保存共用的依赖，使用一组子容器保存每个被测类特有的依赖。
	 * 
	 * @author Rainy
	 *
	 */
    public class BDTestContext : IDisposable
    {
        public static WindsorContainer WindsorContainer { get; } = new WindsorContainer();

        public static Dictionary<Type, WindsorContainer> SubContainers { get; } = new Dictionary<Type, WindsorContainer>();

        public static WindsorContainer SubContainer<TTestee>()
        {
            if (SubContainers.ContainsKey(typeof(TTestee))) return SubContainers[typeof(TTestee)];
            var newsub = new WindsorContainer();
            SubContainers.Add(typeof(TTestee), newsub);
            WindsorContainer.AddChildContainer(newsub);
            return newsub;
        }

        public BDTestContext()
        {
            CastleContainer.WindsorContainer = WindsorContainer;
            registerSharedServices();
        }

        //BD单元测试中大部分服务类都使用替身，但由框架定义的数据库访问功能的类使用实际类，如GenericDAO。
        private void registerSharedServices()
        {
            CastleWindorComponentRegistrar.AddGenericDaosTo(WindsorContainer);

            var container = WindsorContainer;

            container.Register(Component.For(typeof(IUtilQuery)).ImplementedBy(typeof(UtilQuery)));
            container.Register(Component.For(typeof(IUtil)).ImplementedBy(typeof(UtilFake)));

            MockRegistry.RegisterMockDependency<IExceptionTranslator>(() => new Mock<IExceptionTranslator>().Object);
            MockRegistry.RegisterMockDependency<IApplicationStorage>(() => new Mock<IApplicationStorage>().Object);
            MockRegistry.RegisterMockDependency<ITransactionHelper>(() => new Mock<ITransactionHelper>().Object);
            MockRegistry.RegisterMockDependency<IDbContext>(() => new Mock<IDbContext>().Object);
        }

        public void Dispose()
        {
            AbstractDbTestBase.CloseTestDB();
            WindsorContainer.Dispose();
        }

    }

}