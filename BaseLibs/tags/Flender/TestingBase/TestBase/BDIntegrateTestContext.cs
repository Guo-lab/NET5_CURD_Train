using System;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using E2biz.Utility.General;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProjectBase.Application;
using ProjectBase.BusinessDelegate;
using ProjectBase.Desktop;
using ProjectBase.Domain;
using ProjectBase.Utils;
using TestingBase.TestDouble.ProjectBase;

namespace TestingBase.TestBase
{
    /**
	 * BD集成测试运行的Context，Ioc容器相关对象注册。
	 * 
	 * @author Rainy
	 *
	 */
    public class BDIntegrateTestContext : IDisposable
    {
        public static WindsorContainer WindsorContainer { get; } = new WindsorContainer();

        public BDIntegrateTestContext()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            CastleContainer.WindsorContainer = WindsorContainer;
            registerSharedServices();
        }

        //BD集成测试中大部分服务类都使用实际类，仅个别类使用替身
        private void registerSharedServices()
        {
            WindsorContainer.Register(
                Component.For(typeof(IUtil)).ImplementedBy(typeof(UtilFake)).IsDefault(),
                Component.For(typeof(IApplicationStorage)).ImplementedBy(typeof(SimpleApplicationStorage)),
                Component.For<IDateService>().ImplementedBy<TestDouble.MockDateService>()
            );
        }

        public void Dispose()
        {
            AbstractDbTestBase.CloseTestDB();
            WindsorContainer.Dispose();
        }

        public static void RegisterMemoryCache()
        {
            var sp = new ServiceCollection().AddMemoryCache().BuildServiceProvider();
            var ins = sp.GetService<IMemoryCache>();
            WindsorContainer.Register(
                Component.For<IMemoryCache>().Instance(ins)
            );
        }
    }

}