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
		/// <summary>
		/// 获取已添加的所有任务
		/// </summary>
		/// <param name="factoryKey"></param>
		/// <returns></returns>
		IList<TransactionTask> GetTasks(String factoryKey);
		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="factoryKey"></param>
		/// <param name="task"></param>
		void AddTask(String factoryKey, TransactionTask task);

		void BeginTrans(String factoryKey);

		void EndTrans(String factoryKey);

		/// <summary>
		/// 标记当前线程中的SessionStorage废弃，后续将使用新建的SessionStorage。一般web环境下需要此标志表示异步线程不与请求线程在同一线程。
		/// </summary>
		void MarkSessionStorageAbandoned();

		/// <summary>
		/// 异步任务排队等待执行
		/// </summary>
		/// <param name="factoryKey"></param>
		/// <param name="tasks"></param>
		void QueueAsyncTasks(string factoryKey,IList<TransactionTask> tasks);
		/// <summary>
		/// 获取所有排队等待执行的异步任务
		/// </summary>
		/// <param name="factoryKey"></param>
		/// <returns></returns>
		IList<TransactionTask> GetQueuedAsyncTasks(string factoryKey);
	}
}
