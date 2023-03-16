using System;
using System.Collections.Generic;

namespace ProjectBase.Domain.Transaction
{

	/**
	 * 每个 session中可能有多个事务，每个事务都可以关联一组任务。任务在事务的某个时间点执行，如事务提交后。
	 * @see --internal
	 *
	 */
	public interface ITransactionTaskStorage
	{

		IList<TransactionTask> GetTasks(String factoryKey);

		void AddTask(String factoryKey, TransactionTask task);

		void BeginTrans(String factoryKey);

		void EndTrans(String factoryKey);
	}
}
