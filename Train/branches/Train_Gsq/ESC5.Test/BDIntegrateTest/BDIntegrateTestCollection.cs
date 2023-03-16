using TestingBase.TestBase;
using Xunit;

namespace ESC5.Test.BDIntegrateTest
{
    /// <summary>
    /// 定义为一个测试集执行一次的setup和dispose工作。集成测试一般是AppSetup和数据库加载。
    /// 对应产品代码的Startup类
    /// </summary>
    public class BDIntegrateTestStartup : BaseTestApplication
    {
        public BDIntegrateTestStartup():base(new TestAppCommonSetup(), BDIntegrateTestContext.WindsorContainer)
        {
        }

        protected override string[]? GetDataFilesToLoad()
        {
            return new[] { "ESC_Master.xlsx" };
        }
        protected override void Application_Start()
        {
        }
    }

    [CollectionDefinition("BDIntegrate")]
    public class BDIntegrateTestCollection : ICollectionFixture<BDIntegrateTestContext>, ICollectionFixture<BDIntegrateTestStartup>, IClassFixture<PerClassContext>
    {
        // 此类仅用于配合 [CollectionDefinition("xxx")]标记来定义测试集，无任何内部代码，也不会创建实例
    }

}
