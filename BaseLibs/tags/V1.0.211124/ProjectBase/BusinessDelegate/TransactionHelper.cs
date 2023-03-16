using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Data;
using ProjectBase.Domain.Transaction;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;

namespace ProjectBase.BusinessDelegate
{

    public class TransactionHelper : ITransactionHelper
    {

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

        public void DoInTrans(Action codeToRun)
        {
            innerHelper.DoInTrans<string>(() =>
            {
                codeToRun.Invoke();
                return null;
            });
        }

        public T DoInTrans<T>(Func<T> codeToRun)
        {
            return innerHelper.DoInTrans(codeToRun);
        }

        public void RunTasks(IList<TransactionTask> tasks, TransactionTask.TaskRunOnEnum runOn)
        {
            if (tasks == null || tasks.Count == 0)
                return;
            var matched = tasks.Where((task) => task.RunOn == runOn);
            foreach (var task in matched)
            {
                if (task.Async)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        task.Task.Invoke();
                        NHibernateSession.CloseAllSessions();
                    });
                }
                else
                {
                    task.Task.Invoke();
                }
            };
        }

        abstract class InnerHelper
        {
            public IDbContext dbContext { get; set; }
            public TransactionHelper callerHelper { get; set; }
            public abstract void AddTask(String factoryKey, TransactionTask task);

            public abstract T DoInTrans<T>(Func<T> codeToRun);

            protected T internalDoInTrans<T>(Func<T> codeToRun, String factoryKey, ITransactionTaskStorage storage)
            {
                dbContext.BeginTransaction();
                storage.BeginTrans(factoryKey);
                IList<TransactionTask> tasks = null;
                T rtn = default(T);
                try
                {
                    rtn = codeToRun.Invoke();
                }
                catch (Exception)
                {
                    dbContext.RollbackTransaction();
                    tasks = storage.GetTasks(factoryKey);
                    storage.EndTrans(factoryKey);
                    // TODO 是否抛出异常由consumer决定
                    callerHelper.RunTasks(tasks, TransactionTask.TaskRunOnEnum.OnActionFail);
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
                if (commitErr == null)
                {
                    callerHelper.RunTasks(tasks, TransactionTask.TaskRunOnEnum.AfterCommit);
                    return rtn;
                }
                else
                {
                    // TODO 是否抛出异常由consumer决定
                    callerHelper.RunTasks(tasks, TransactionTask.TaskRunOnEnum.OnCommitFail);
                    throw commitErr;
                }
            }
        }

        class HbInnerHelper : InnerHelper
        {
            public override void AddTask(String factoryKey, TransactionTask task)
            {
                ITransactionTaskStorage storage = (ITransactionTaskStorage)NHibernateSession.Storage;
                storage.AddTask(factoryKey, task);
            }

            public override T DoInTrans<T>(Func<T> codeToRun)
            {
                String factoryKey = SessionFactoryKeyHelper.GetKey();
                ((DbContext)dbContext).FactoryKey=factoryKey;
                ITransactionTaskStorage storage = (ITransactionTaskStorage)NHibernateSession.Storage;
                return internalDoInTrans(codeToRun, factoryKey, storage);
            }
        }


    }
}