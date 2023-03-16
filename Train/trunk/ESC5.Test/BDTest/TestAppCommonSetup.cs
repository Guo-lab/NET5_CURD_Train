using Castle.MicroKernel.Registration;
using ESC5.Common;
using IdentityService;
using ProjectBase.Data;
using TestingBase.TestDouble.IdentityService;

namespace ESC5.Test.BDTest
{
    //测试Context启动过程尽量使用产品代码，如有不同，在子类中重写。
    //单元测试一般需覆盖SetupCommonFeature()
    //注意在CustomContainerRegister方法中添加必要的IOC注册
    public class TestAppCommonSetup : AppCommonSetup
    {
        public override void SetupCommonFeature()
        {
            CustomContainerRegister();
            //NHibernateSession.RegisterInterceptor(new MyInterceptor());
            NHibernateSessionModified.AfterInit = AfterNHInit;
        }
        protected override void CustomContainerRegister()
        {
            WindsorContainer.Register(
               Component.For<IIdentity>().ImplementedBy<MockIdentity>().LifestyleTransient()
               //Component.For<IFiscalPeriodBD>().ImplementedBy<FiscalPeriodBD>()//此类使用实际类
            );
        }
    }
}
