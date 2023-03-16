using NHibernate.Tool.hbm2ddl;
using ProjectBase.Application;
using SharpArch.NHibernate;
namespace TestingBase.TestBase
{
    /// <summary>
    ///     Provides a base class for running unit tests against an in-memory database created
    ///     during test execution.  This builds the database using the connection details within
    ///     NHibernate.config.  If you'd prefer unit testing against a "live" development database
    ///     such as a SQL Server instance, then use <see cref = "DatabaseRepositoryTestsBase" /> instead.
    ///     If you'd prefer a more behavior driven approach to testing against the in-memory database,
    ///     use <see cref = "RepositoryBehaviorSpecificationTestsBase" /> instead.
    /// </summary>
    public abstract class RepositoryTestBase
    {
        public IAppCommonSetup AppCommonSetup { get; set; }
        [TearDown]
        public virtual void TearDown()
        {
            RepositoryTestsHelper.Shutdown();
        }

        protected void FlushSessionAndEvict(object instance)
        {
            RepositoryTestsHelper.FlushSessionAndEvict(instance);
        }

        protected abstract void LoadTestData();

        protected void LoadExcelData(string filePath)
        {
            IToSqliteMemory excelToSqlite = new ExcelToSqlite(TestContext.CurrentContext.TestDirectory + "\\" + filePath);
            excelToSqlite.ToSqliteMemory();
        }

        [SetUp]
        public virtual void SetUp()
        {
            var cfg = this.AppCommonSetup.InitializeNHibnerate(new SimpleSessionStorage());
            var connection = NHibernateSession.Current.Connection;
            new SchemaExport(cfg).Execute(false, true, false, connection, null);
            this.LoadTestData();
        }
    }
}