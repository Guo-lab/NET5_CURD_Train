using System;
using System.Collections.Generic;

namespace ProjectBase.Domain.Transaction
{

    /**
	 * 统一{@link projectbase.mvc.Transaction}标记方式定义事务(可添加任务)和编程方式自定义事务(可添加任务)，两种方式原理一致。
	 * @author Rainy
	 *
	 */
    public interface ITransactionHelper
    {
        /**
		 * 添加任务，该任务对应{@link projectbase.mvc.Transaction}标记的设置，在其标记的事务外执行。
		 * @param task
		 */
        void AddTask(TransactionTask task);
        /**
		 *  添加任务，该任务对应{@link projectbase.mvc.Transaction}标记的设置，在其标记的事务外执行。
		 * @param factoryKey
		 * @param task
		 */
        void AddTask(String factoryKey, TransactionTask task);
        /**
		 *  添加同步任务，该任务对应{@link projectbase.mvc.Transaction}标记的设置，在其标记的事务外执行。
		 * @param task
		 */
        void AddSyncTask(Action task);
        /**
		 *  添加异步任务，该任务对应{@link projectbase.mvc.Transaction}标记的设置，在其标记的事务外执行。
		 * @param task
		 */
        void AddAsyncTask(Action task);
        /**
		 * 将参数指定的代码作为一个事务执行
		 * @param codeToRun
		 */
        void DoInTrans(Action codeToRun);
        /**
         * 将参数指定的代码作为一个事务执行
         * @param codeToRun
         */
        T DoInTrans<T>(Func<T> codeToRun);

        void RunTasks(IList<TransactionTask> tasks, TransactionTask.TaskRunOnEnum runOn);

    }
}