using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ProjectBase.Data;
using ProjectBase.Domain.Transaction;
using ProjectBase.Utils;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Web.Mvc;

namespace ProjectBase.BusinessDelegate
{
    /// <summary>
    /// 事务有两种定义方式，一种使用Transaction标记，另一种是TransactionHelper.DoInTrans
    /// Transaction标记将异步任务在事务处理的最后执行，因为就一个事务。
    /// TransactionHelper.DoInTrans可能被多次调用，即多个事务，因此多个事务的所有要执行的异步任务都先排队，等合适的时机再执行
    /// </summary>
    public class TransactionHelper : ITransactionHelper
    {
        public IUtil Util { get; set; }

        private InnerHelper innerHelper;

        public TransactionHelper(IDbContext dbContext)
        {
            innerHelper = new HbInnerHelper()
            {
                dbContext = dbContext,
                callerHelper = this
            };
        }

        public void AddTask(TransactionTask task)
        {
            String factoryKey = SessionFactoryKeyHelper.GetKey();
            AddTask(factoryKey, task);
        }

        public void AddTask(String factoryKey, TransactionTask task)
        {
            innerHelper.AddTask(factoryKey, task);
        }

        public void AddSyncTask(Action task)
        {
            AddTask(new TransactionTask()
            {
                Async = false,
                Task = task
            });
        }

        public void AddAsyncTask(Action task)
        {
            AddTask(new TransactionTask()
            {
                Task = task
            });
        }

        public void DoInTrans(Action codeToRun, bool closeSession = false)
        {
            innerHelper.DoInTrans<string>(() =>
            {
                codeToRun.Invoke();
                return null;
            }, closeSession);
        }

        public T DoInTrans<T>(Func<T> codeToRun, bool closeSession = false)
        {
            return innerHelper.DoInTrans(codeToRun, closeSession);
        }
        public void RunTasks(IList<TransactionTask> tasks, TransactionTask.TaskRunOnEnum runOn)
        {
            RunSyncTasks(tasks, runOn);
            RunAsyncTasks(tasks, runOn);
        }
        public void RunSyncTasks(IList<TransactionTask> tasks, TransactionTask.TaskRunOnEnum runOn)
        {
            if (tasks == null || tasks.Count == 0)
                return;
            //所有同步任务按顺序执行
            foreach (var task in tasks.Where((task) => task.RunOn == runOn && !task.Async))
            {
                task.Task.Invoke();
            }
        }
        public void RunAsyncTasks(IList<TransactionTask> tasks, TransactionTask.TaskRunOnEnum runOn)
        {
            RunAsyncTasks(tasks.Where((task) => task.RunOn == runOn && task.Async).ToList());
        }
        public void RunQueuedAsyncTasks(string factoryKey)
        {
            var tasks=innerHelper.GetQueuedAsyncTasks(factoryKey);
            innerHelper.MarkSessionStorageAbandoned();//这句必须在这里。上一句还需要用到当前SessionStorage，用完后这句就可以abandon了。接下来的异步任务再访问SessionStorage就是新的一个了.
            RunAsyncTasks(tasks);
        }
        private void RunAsyncTasks(IList<TransactionTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
                return;
            foreach (var task in tasks)
            {
                Task.Run(() =>
                {
                    NHibernateSession.Storage.BeginUsageOutofRequestContext();//标记HBSession在脱离了Http上下文的线程中使用
                    try
                    {
                        task.Task.Invoke();
                    }
                    catch (Exception e)
                    {
                        Util.AddLog("Async Task Error", e);
                    }
                    finally
                    {
                        NHibernateSession.CloseAllSessions();
                    }
                });
            }
        }

        abstract class InnerHelper
        {
            public IDbContext dbContext { get; set; }
            public TransactionHelper callerHelper { get; set; }
            public abstract void AddTask(String factoryKey, TransactionTask task);
            public abstract IList<TransactionTask> GetTasks(String factoryKey);
            public abstract T DoInTrans<T>(Func<T> codeToRun, bool closeSession = false);

            /// <summary>
            /// 标记当前线程中的SessionStorage废弃，后续将使用新建的SessionStorage。一般web环境下需要此标志表示异步线程不与请求线程在同一线程。
            /// </summary>
            public virtual void MarkSessionStorageAbandoned() { }
            public abstract IList<TransactionTask> GetQueuedAsyncTasks(string factoryKey);

            protected T internalDoInTrans<T>(Func<T> codeToRun, string factoryKey, ITransactionTaskStorage storage, bool closeSession = false)
            {
                dbContext.BeginTransaction();
                storage.BeginTrans(factoryKey);
                IList<TransactionTask> tasks = null;
                TransactionTask.TaskRunOnEnum runOn;
                T rtn = default(T);
                try
                {
                    rtn = codeToRun.Invoke();
                }
                catch (Exception)
                {
                    dbContext.RollbackTransaction();
                    if (closeSession)
                    {
                        this.closeSession(factoryKey);
                    }
                    tasks = storage.GetTasks(factoryKey);
                    storage.EndTrans(factoryKey);
                    // TODO 是否抛出异常由consumer决定
                    runOn = TransactionTask.TaskRunOnEnum.OnActionFail;
                    runTasks(tasks, runOn, storage, factoryKey);
                    throw;
                }

                tasks = storage.GetTasks(factoryKey);
                storage.EndTrans(factoryKey);
                Exception commitErr = null;
                try
                {
                    dbContext.CommitTransaction();
                }
                catch (Exception e)
                {
                    dbContext.RollbackTransaction();
                    commitErr = e;
                }
                if (closeSession)
                {
                    this.closeSession(factoryKey);
                }
                if (commitErr == null)
                {
                    runOn = TransactionTask.TaskRunOnEnum.AfterCommit;
                    runTasks(tasks, runOn, storage, factoryKey);
                    return rtn;
                }
                else
                {
                    // TODO 是否抛出异常由consumer决定
                    runOn = TransactionTask.TaskRunOnEnum.OnCommitFail;
                    runTasks(tasks, runOn, storage, factoryKey);
                    throw new Exception("提交事务时发生异常",commitErr);
                }

            }

            private void runTasks(IList<TransactionTask> tasks, TransactionTask.TaskRunOnEnum runOn, ITransactionTaskStorage storage, String factoryKey)
            {
                callerHelper.RunSyncTasks(tasks, runOn);
                //自定义事务将所有要执行的异步任务先排队
                storage.QueueAsyncTasks(factoryKey,tasks.Where(o => o.Async && o.RunOn== runOn).ToList());
            }

            private void closeSession(string factoryKey)
            {
                NHibernateSession.CloseAllSessions();
                NHibernateSession.Storage.RemoveSessionForKey(factoryKey);
            }
        }

        class HbInnerHelper : InnerHelper
        {
            public override void AddTask(String factoryKey, TransactionTask task)
            {
                ITransactionTaskStorage storage = (ITransactionTaskStorage)NHibernateSession.Storage;
                storage.AddTask(factoryKey, task);
            }
            public override IList<TransactionTask> GetTasks(String factoryKey)
            {
                ITransactionTaskStorage storage = (ITransactionTaskStorage)NHibernateSession.Storage;
                return storage.GetTasks(factoryKey);
            }

            public override T DoInTrans<T>(Func<T> codeToRun, bool closeSession = false)
            {
                String factoryKey = SessionFactoryKeyHelper.GetKey();
                ((DbContext)dbContext).FactoryKey = factoryKey;
                ITransactionTaskStorage storage = (ITransactionTaskStorage)NHibernateSession.Storage;
                return internalDoInTrans(codeToRun, factoryKey, storage, closeSession);
            }

            public override void MarkSessionStorageAbandoned()
            {
                ITransactionTaskStorage storage = (ITransactionTaskStorage)NHibernateSession.Storage;
                storage.MarkSessionStorageAbandoned();
            }
            public override IList<TransactionTask> GetQueuedAsyncTasks(string factoryKey)
            {
                ITransactionTaskStorage storage = (ITransactionTaskStorage)NHibernateSession.Storage;
                return storage.GetQueuedAsyncTasks(factoryKey);
            }
        }


        /// <summary>
        /// 在application范围内的打开和关闭session，期间执行hibernate操作。
        /// </summary>
        /// <param name="action">包含要执行的hibernate操作，session的打开关闭是自动的。</param>
        public static void DoHbSessionInAppScope(Action action)
        {
            action.Invoke();
            NHibernateSession.CloseAllSessions();
        }

        public void FlushAndClearSession()
        {
            FlushAndClearSession(SessionFactoryKeyHelper.GetKey());
        }

        public void FlushAndClearSession(string factoryKey)
        {
            var session = NHibernateSession.CurrentFor(factoryKey);
            session.Flush();
            session.Clear();
        }
    }
}
