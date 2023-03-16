using System;
using System.Collections.Generic;
using SharpArch.NHibernate;

namespace ProjectBase.Domain.Transaction
{

    /**
	 * 类{@link SimpleHibernateSessionStorage}的扩展，在保存hibernate session同时还保存相关的一组{@link TransactionTask}
	 * @see --internal
	 *
	 */
    public class SimpleHibernateSessionTaskStorage : SimpleSessionStorage, ITransactionTaskStorage
    {
        private TransactionTaskStorageImpl store = new TransactionTaskStorageImpl();

        public IList<TransactionTask> GetTasks(String factoryKey)
        {
            return store.GetTasks(factoryKey);
        }

        public void AddTask(String factoryKey, TransactionTask task)
        {
            store.AddTask(factoryKey, task); ;
        }

        public void BeginTrans(String factoryKey)
        {
            store.BeginTrans(factoryKey);
        }

        public void EndTrans(String factoryKey)
        {
            store.EndTrans(factoryKey);
        }

        public void QueueAsyncTasks(string factoryKey, IList<TransactionTask> tasks)
        {
            store.QueueAsyncTasks(factoryKey, tasks);
        }

        public IList<TransactionTask> GetQueuedAsyncTasks(string factoryKey)
        {
            return store.GetQueuedAsyncTasks(factoryKey);
        }

        public void MarkSessionStorageAbandoned()
        {
            throw new NotImplementedException();
        }
    }
}
