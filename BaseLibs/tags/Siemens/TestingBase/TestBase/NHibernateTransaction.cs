using System;
using NHibernate;
using SharpArch.NHibernate;

namespace TestingBase.TestBase
{

	/**
	 * BD测试时被测方法在hibernate transaction中进行
	 * @author Rainy
	 * @see --internal
	 */
	public class NHibernateTransaction
	{
		static private string factoryKey;
		static ITransaction tx;

		static void SetFactoryKey(string sessionFactoryKey)
		{
			factoryKey = sessionFactoryKey;
		}
		public static ITransaction Current()
		{
			return CurrentFor(null);
		}
		public static ITransaction CurrentFor(string sessionFactoryKey)
		{
			if (tx == null || !tx.IsActive)
				tx = NHibernateSession.CurrentFor(GetEffectiveFactoryKey(factoryKey)).BeginTransaction();
			return tx;
		}

		public static void DoInHibernate(Action f)
		{
			DoInHibernate(f, false);
		}
		public static void DoInHibernate(Action f, bool commit)
		{
			tx = NHibernateSession.CurrentFor(GetEffectiveFactoryKey(factoryKey)).BeginTransaction();
			try
			{
				f.Invoke();
				if (tx.IsActive)
				{
					if (commit)
					{
						tx.Commit();
					}
					else
					{
						tx.Rollback();
					}
				}
			}
			catch (Exception)
			{
				if (tx.IsActive) tx.Rollback();
			}
		}
		private static string GetEffectiveFactoryKey(string factoryKey)
		{
			return string.IsNullOrEmpty(factoryKey) ? SessionFactoryKeyHelper
					.GetKey() : factoryKey;
		}

	}
}
