using TestingBase.TestBase;
using Xunit;
using static TestingBase.TestBase.BDTestContext;

namespace ESC5.Test.BDTest
{
    /// <summary>
    /// 定义为一个测试集执行一次的setup和dispose工作。一般是AppSetup和数据库加载。
    /// 对应产品代码的Startup类
    /// </summary>
    public class BDTestStartup : BaseTestApplication
    {
        public BDTestStartup() : base(new TestAppCommonSetup(), WindsorContainer)
        {
        }
        protected override string[]? GetDataFilesToLoad()
        {
            return new[] { "ESC_Master.xlsx" };
        }
        protected override void Application_Start()
        {
            //一般不需要自定义加载过程，见GetDataFilesToLoad);
            //AbstractDbTestBase.LoadSqlData("GN_User", 
            //    "PP_PlanningCommodity", 
            //    "PP_PlanningCommodityPart",
            //    "PP_LeadTime",
            //    "PP_TransportTime",
            //    "PP_InspectTime",
            //    "PP_FiscalPeriod");
        }

    }

    [CollectionDefinition("BD")]
    public class BDTestCollection : ICollectionFixture<BDTestContext>, ICollectionFixture<BDTestStartup>, IClassFixture<PerClassContext>
    {
        // 此类仅用于配合 [CollectionDefinition("xxx")]标记来定义测试集，无任何内部代码，也不会创建实例
    }

}
