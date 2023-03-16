using Castle.Windsor;
using ProjectBase.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingBase.TestBase
{
    /// <summary>
    /// 定义为一个测试集执行一次的setup和dispose工作。包括AppSetup和数据库加载。
    /// 对应产品代码的BaseApplication
    /// </summary>
    public class BaseTestApplication
    {
        public BaseTestApplication(IAppCommonSetup setup, IWindsorContainer container)
        {
            setup.WindsorContainer = container;
            setup.SetupCommonFeature();
            AbstractDbTestBase.SetUpTestDB(setup, "db", GetDataFilesToLoad());
            Application_Start();
        }
        protected virtual void Application_Start()
        {
        }
        protected virtual string[]? GetDataFilesToLoad()
        {
            return null;
        }
    }
}
