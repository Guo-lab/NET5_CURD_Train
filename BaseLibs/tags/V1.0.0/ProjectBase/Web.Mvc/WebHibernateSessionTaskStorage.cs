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

		protected override ThreadLocal<SimpleSessionStorage> GetInitValueOfStorageForTaskThread()
		{
			return new ThreadLocal<SimpleSessionStorage>(() => new SimpleHibernateSessionTaskStorage());
		}
	}
}
