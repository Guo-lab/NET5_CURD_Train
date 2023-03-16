using System;
using System.Linq;
using NHibernate;
using ProjectBase.Domain;
using SharpArch.NHibernate;
using Xunit.Abstractions;

namespace TestingBase.TestBase
{
    /**
	 * BD集成测试类基类
	 *   1. 加载环境上下文
	 *   2. 每个测试方法后关闭hibernate session。
	 *   3. 记录tesrunlog
	 *   4.使用{@link HibernateTransaction}管理事务。由于hibernate需要commit才能与数据库同步，才能跟测试用的jdbc操作的数据共享状态，
	 *   因此不适合用springtransactional测试方法，而是模仿其行为，将每个测试方法包在hibernate的transaction中，方法后rollback。
	 * @author Rainy
	 * @see --basic
	 */
    /////////@Listeners({TestRunLogListener.class,HibernateSuiteListener.class})
    public abstract class BDIntegrateTestBase : AbstractDbTestBase, IDisposable
    {
        protected readonly ITestOutputHelper output;

        protected ITransaction tx;

        protected BDTestContext context;

        protected PerClassContext perClassContext;

        public BDIntegrateTestBase(PerClassContext perClassContext = null, ITestOutputHelper output = null, BDTestContext context = null)
        {
            this.output = output;
            this.context = context;
            this.perClassContext = perClassContext;
            if (perClassContext != null && !perClassContext.DoneSetup)
            {
                BeforeClass();
                perClassContext.DoneSetup = true;
                perClassContext.DisposeAction = () => AfterClass();
            }
            SuperBeforeMethod();
            BeforeMethod();
        }

        protected virtual void BeforeClass()
        {
            // Load Test Data
        }
        protected virtual void AfterClass()
        {
            //clear data
        }
        protected virtual void SuperBeforeMethod()
        {
            OpenTransaction();
        }
        protected virtual void BeforeMethod()
        {
        }
        private void OpenTransaction()
        {
            if (BeforeTransaction())
            {
                tx = NHibernateTransaction.Current();
            }
        }

        private void CloseTransaction()
        {
            if (tx.IsActive) tx.Rollback();
            AfterTransaction();
            CloseSession();
        }
        private void CloseSession()
        {
            NHibernateSession.CloseAllSessions();
            NHibernateSession.Storage = new SimpleSessionStorage();
        }
        /**
         * 在自动开始事务前进行准备工作
         * @return true-开始事务，false-不开始事务。如，此方法中已开始事务，则返回false，指示基类不再开始事务。
         */
        protected virtual bool BeforeTransaction()
        {
            return true;
        }
        /**
         * 在事务完成后进行工作
         */
        protected virtual void AfterTransaction()
        {
        }
        /**
         * 在hibernate transaction中执行hibernate相关动作
         * @param f
         */
        public void DoInHibernate(Action f, bool commit)
        {
            NHibernateTransaction.DoInHibernate(f, commit);
        }
        public void DoInHibernate(Action f)
        {
            NHibernateTransaction.DoInHibernate(f);
        }
        public static T Resolve<T>()
        {
            return BDIntegrateTestContext.WindsorContainer.Resolve<T>();
        }
        public static T[] ResolveAll<T>()
        {
            return BDIntegrateTestContext.WindsorContainer.ResolveAll<T>();
        }
        public static T Resolve<IT,T>()
        {
            IT[] all= BDIntegrateTestContext.WindsorContainer.ResolveAll<IT>();
            return all.OfType<T>().SingleOrDefault();
        }
        public virtual void Dispose()
        {
            CloseTransaction();
        }
        //获得Dao对象以便访问测试数据库
        public static IGenericDaoWithTypedId<T, TId> GetDao<T, TId>() where T : BaseDomainObjectWithTypedId<TId> where TId : struct
        {
            return BDIntegrateTestContext.WindsorContainer.Resolve<IGenericDaoWithTypedId<T, TId>>();
        }
    }
}