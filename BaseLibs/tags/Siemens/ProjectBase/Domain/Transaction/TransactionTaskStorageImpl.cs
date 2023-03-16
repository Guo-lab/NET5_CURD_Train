using SharpArch.Domain;
using System;
using System.Collections.Generic;

namespace ProjectBase.Domain.Transaction
{

    /**
	 * 类{@link SimpleHibernateSessionStorage}的扩展，在保存hibernate session同时还保存其下每个打开的事务相关的一组{@link TransactionTask}
	 * @see --internal
	 *
	 */
    public class TransactionTaskStorageImpl : ITransactionTaskStorage
    {
        //factoryKey mapTo session，每个session有一个transStack，每个trans又包含一个taskList
        private IDictionary<String, Stack<IList<TransactionTask>>> sessionMap = new Dictionary<String, Stack<IList<TransactionTask>>>();
        //factoryKey mapTo session，每个session有一个异步任务List.异步任务需要先记录下来等合适的时机再执行
        private IDictionary<String, IList<TransactionTask>> sessionAysncTaskQueueMap = new Dictionary<String, IList<TransactionTask>>();

        public void BeginTrans(String factoryKey)
        {
            Stack<IList<TransactionTask>> transStack;
            if (sessionMap.ContainsKey(factoryKey))
            {
                transStack = sessionMap[factoryKey];
            }
            else
            {
                transStack = new Stack<IList<TransactionTask>>();
                sessionMap[factoryKey] = transStack;
                sessionAysncTaskQueueMap[factoryKey] = new List<TransactionTask>();
            }
            transStack.Push(new List<TransactionTask>());
        }

        public void EndTrans(String factoryKey)
        {
            var transStack = sessionMap[factoryKey];
            transStack.Pop();
        }

        public IList<TransactionTask> GetTasks(String factoryKey)
        {
            if (!sessionMap.ContainsKey(factoryKey))
            {
                return null;
            }
            var transStack = sessionMap[factoryKey];
            return transStack.Peek();
        }

        public void AddTask(String factoryKey, TransactionTask task)
        {
            Check.Require(sessionMap.ContainsKey(factoryKey), "TransactionTask添加异常，请检查是否在事务中");

            var transStack = sessionMap[factoryKey];
            transStack.Peek().Add(task);
        }
        public void QueueAsyncTasks(String factoryKey, IList<TransactionTask> tasks)
        {
            foreach (TransactionTask task in tasks)
            {
                sessionAysncTaskQueueMap[factoryKey].Add(task);
            }
        }
        public IList<TransactionTask> GetQueuedAsyncTasks(string factoryKey)
        {
            if (!sessionMap.ContainsKey(factoryKey))
            {
                return null;
            }
            return sessionAysncTaskQueueMap[factoryKey];
        }

        public void MarkSessionStorageAbandoned()
        {
            throw new NotImplementedException();
        }

        public bool IsInTrans(string factoryKey)
        {
            return sessionMap.ContainsKey(factoryKey);
        }
    }
}
