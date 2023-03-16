using System;
using Castle.MicroKernel.Registration;
using Moq;
using NHibernate;
using ProjectBase.Domain;
using SharpArch.NHibernate;
using Xunit;
using Xunit.Abstractions;
using static TestingBase.TestBase.BDTestContext;

namespace TestingBase.TestBase
{
    /**
	 * BD测试类基类
	 *   1. 加载环境上下文
	 *   2. 每个测试方法后关闭hibernate session。
	 *   3. 记录tesrunlog
	 *   4.使用{@link HibernateTransaction}管理事务。由于hibernate需要commit才能与数据库同步，才能跟测试用的jdbc操作的数据共享状态，
	 *   因此不适合用springtransactional测试方法，而是模仿其行为，将每个测试方法包在hibernate的transaction中，方法后rollback。
	 *   5.为被测类的依赖自动创建Mock对象：对未经父容器解析或手动解析的依赖，自动创建Mock对象
	 * @author Rainy
	 * @see --basic
	 */
    /////////@Listeners({TestRunLogListener.class,HibernateSuiteListener.class})
    public abstract class BDTestBase : AbstractDbTestBase, IDisposable
    {
        protected readonly ITestOutputHelper output;

        protected ITransaction tx;

        protected BDTestContext context;

        protected PerClassContext perClassContext;

        private static Func<object> getTesteeAction;

        private static Func<object,object> autoDITesteeAction;

        public BDTestBase(PerClassContext perClassContext = null, ITestOutputHelper output = null, BDTestContext context = null)
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
        protected abstract void OnSetTestee(object value);

        protected virtual void BeforeMethod()
        {
            var testee = getTesteeAction.Invoke();
            OnSetTestee(testee);
            autoDITesteeAction.Invoke(testee);
            OpenTransaction();
        }
        protected virtual void AfterMethod()
        {
        }
        private void OpenTransaction()
        {
            output?.WriteLine("before");
            if (BeforeTransaction())
            {
                tx = NHibernateTransaction.Current();
            }
        }

        protected void CloseTransaction()
        {
            output?.WriteLine("after");
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
        public virtual void Dispose()
        {
            AfterMethod();
            CloseTransaction();
        }

        public static TTestee MockDITestee<TService, TTestee>(TTestee testee) where TTestee : class, TService
        {
            var props = typeof(TTestee).GetProperties();
            foreach (var prop in props)
            {
                if (prop.CanWrite && prop.PropertyType.IsInterface && prop.GetValue(testee) == null)
                {
                    if (MockRegistry.NeedMock(prop.PropertyType))
                    {
                        var mockObj= MockRegistry.GetMockObj(prop.PropertyType);
                        prop.SetValue(testee, mockObj);
                    }
                    else if(IsMockable(prop.PropertyType))// if (typeof(IBusinessDelegate).IsAssignableFrom(prop.PropertyType))
                    {
                        var propMockType = typeof(Mock<>).MakeGenericType(prop.PropertyType);
                        Mock propMock = (Mock)Activator.CreateInstance(propMockType);
                        prop.SetValue(testee, propMock.Object);
                    }
                }
            }
            return testee;

        }
        public static void RegTestee<TService, TTestee>() where TTestee : class, TService
        {
            if (SubContainers.ContainsKey(typeof(TTestee))) return;
            var sub = SubContainer<TTestee>();
            sub.Register(Component.For(typeof(TService)).ImplementedBy<TTestee>().LifestyleTransient());
            autoDITesteeAction = (testee) => MockDITestee<TService, TTestee>(testee as TTestee);
            getTesteeAction = () => SubContainer<TTestee>().Resolve<TService>() as TTestee; 
        }

        public static bool IsMockable(Type clazz)
        {
            return !clazz.IsSealed && clazz.IsInterface;
        }

        //public ICommonBD<T, TId> GetBD<T, TId>() where T : BaseDomainObjectWithTypedId<TId> where TId : struct
        //{
        //    return WindsorContainer.Resolve<ICommonBD<T, TId>>();
        //}

        //获得Dao对象以便访问测试数据库
        public static IGenericDaoWithTypedId<T, TId> GetDao<T, TId>() where T : BaseDomainObjectWithTypedId<TId> where TId : struct
        {
            return WindsorContainer.Resolve<IGenericDaoWithTypedId<T, TId>>();
        }
    }
}