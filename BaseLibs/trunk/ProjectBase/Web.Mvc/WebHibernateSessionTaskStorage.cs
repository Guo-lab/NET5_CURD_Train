using System;
using System.Collections.Generic;
using System.Threading;
using ProjectBase.Domain.Transaction;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Web.Mvc;

namespace ProjectBase.Web.Mvc
{


	/**
	 * 对{@link WebHibernateSessionStorage}的扩展，添加了对TransactionTask的保存。
	 * @see --advanced
	 */
	public class WebHibernateSessionTaskStorage : WebSessionStorage, ITransactionTaskStorage
	{
		public static new int Application_EndRequest_Order = 99998;
		public override RequestListenerRegistra Registra
		{
			get
			{
				return base.Registra;
			}
			set
			{
				base.Registra = value;
				base.Registra.AddEndHandler(Application_EndRequest, Application_EndRequest_Order);
			}
		}

		protected override SimpleSessionStorage GetNewStorage()
		{
			return new SimpleHibernateSessionTaskStorage();
		}

		public IList<TransactionTask> GetTasks(String factoryKey)
		{
			SimpleHibernateSessionTaskStorage store = (SimpleHibernateSessionTaskStorage)GetSimpleSessionStorage();
			return store.GetTasks(factoryKey);
		}

		public void AddTask(String factoryKey, TransactionTask task)
		{
			SimpleHibernateSessionTaskStorage store = (SimpleHibernateSessionTaskStorage)GetSimpleSessionStorage();
			store.AddTask(factoryKey, task); ;
		}

		public void BeginTrans(String factoryKey)
		{
			SimpleHibernateSessionTaskStorage store = (SimpleHibernateSessionTaskStorage)GetSimpleSessionStorage();
			store.BeginTrans(factoryKey);
		}

		public void EndTrans(String factoryKey)
		{
			SimpleHibernateSessionTaskStorage store = (SimpleHibernateSessionTaskStorage)GetSimpleSessionStorage();
			store.EndTrans(factoryKey);
		}

		//protected override ThreadLocal<SimpleSessionStorage> GetInitValueOfStorageForTaskThread()
		//{
		//	return new ThreadLocal<SimpleSessionStorage>(() => new SimpleHibernateSessionTaskStorage());
		//}
		public void MarkSessionStorageAbandoned()
		{
			if (Hca.HttpContext!=null) {
				Hca.HttpContext.Items[KEY_SESSION_STORAGE_ABANDONED] = true;
			}
		}

        public void QueueAsyncTasks(string factoryKey, IList<TransactionTask> tasks)
        {
			SimpleHibernateSessionTaskStorage store = (SimpleHibernateSessionTaskStorage)GetSimpleSessionStorage();
			store.QueueAsyncTasks(factoryKey, tasks);
		}

        public IList<TransactionTask> GetQueuedAsyncTasks(string factoryKey)
        {
			SimpleHibernateSessionTaskStorage store = (SimpleHibernateSessionTaskStorage)GetSimpleSessionStorage();
			return store.GetQueuedAsyncTasks(factoryKey);
		}
		public bool IsInTrans(string factoryKey)
        {
			SimpleHibernateSessionTaskStorage store = (SimpleHibernateSessionTaskStorage)GetSimpleSessionStorage();
			return store.IsInTrans(factoryKey);
		}
		//请求结束时执行所有排队的异步任务
		public static new void Application_EndRequest(Microsoft.AspNetCore.Http.HttpContext context)
		{
			var factoryKeys = NHibernateSession.Storage.GetAllKeys();
			var hbTransHelper = (ITransactionHelper)context.RequestServices.GetService(typeof(ITransactionHelper));
            foreach (string factoryKey in factoryKeys)
            {
				hbTransHelper.RunQueuedAsyncTasks(factoryKey);
			}
		}
	}
}
