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
                sessionMap[factoryKey]=transStack;
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
            var transStack = sessionMap[factoryKey];
            transStack.Peek().Add(task);
        }

    }
}
