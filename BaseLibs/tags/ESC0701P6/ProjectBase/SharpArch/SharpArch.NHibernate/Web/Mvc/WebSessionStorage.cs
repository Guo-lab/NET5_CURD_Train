using System;
using System.Collections.Generic;
using System.Threading;
using global::NHibernate;
using ProjectBase.BusinessDelegate;
using ProjectBase.Web.Mvc;

namespace SharpArch.NHibernate.Web.Mvc
{
    /**
     * ��webӦ���б���hibernate session��ʵ�ʾ��ǰ�SimpleSessionStorage�ٽ�һ��������request��attribute��ʹ������request�ڼ䶼���á�
     * һ�㱣����request�У�����Ҫʱ��session�����������ִ�е�һ����ѯʱ�򿪣��������ǵ�������ʼʱ�򿪣���request����ʱ�ر�session��ɾ���ռ䡣
     * ����������£���Ӧ������ʱû��request��������Ҫhibernate session����ʱ�Ͳ�������Ӧ��request������������simple session storage�浽һ����̬��ַ�С�
     * ��������¿ͻ����������е���HibernateSession.CloseAllSessions()�Թر�session��
     * @see --advanced
     */
    public class WebSessionStorage : ISessionStorage
    {
        protected const string HttpContextSessionStorageKey = "HttpContextSessionStorageKey";
        public static readonly string KEY_SESSION_STORAGE_ABANDONED = "SessionStorageInThisThreadWasAbandoned";
        public static int Application_EndRequest_Order = 99999;

        private static ThreadLocal<SimpleSessionStorage> storageForTaskThread;
        public Microsoft.AspNetCore.Http.IHttpContextAccessor Hca { get; set; }

        private RequestListenerRegistra registra;
        public virtual RequestListenerRegistra Registra
        {
            get
            {
                return registra;
            }
            set
            {
                registra = value;
                registra.AddEndHandler(Application_EndRequest, Application_EndRequest_Order);
            }
        }

        public IEnumerable<ISession> GetAllSessions()
        {
            var storage = GetSimpleSessionStorage();
            return storage.GetAllSessions();
        }

        public ISession GetSessionForKey(string factoryKey)
        {
            var storage = GetSimpleSessionStorage();
            return storage.GetSessionForKey(factoryKey);
        }

        public void SetSessionForKey(string factoryKey, ISession session)
        {
            var storage = GetSimpleSessionStorage();
            storage.SetSessionForKey(factoryKey, session);
        }

        public virtual SimpleSessionStorage GetSimpleSessionStorage()
        {
            var context = Hca.HttpContext;
            if (context != null && !context.Items.ContainsKey(KEY_SESSION_STORAGE_ABANDONED))
            { //��request��request��ȡ. �����첽�̵߳�ʱ��������request����֮ǰ����ʱ��������requestͬһ�̻߳����ܷ��ʵ�HttpContext��������Ҫ�첽�߳�����ʱ������־���˴�ͨ�������־�ж��첽����ʹ��HttpContext
                var storage = (SimpleSessionStorage)context.Items[HttpContextSessionStorageKey];
                if (storage == null)
                {
                    storage = GetNewStorage();
                    context.Items[HttpContextSessionStorageKey] = storage;
                }
                return storage;
            }
            else
            {//û��request�Ͱ�sessionstorage���ڵ�ǰ�߳���
                if (storageForTaskThread == null)
                {
                    storageForTaskThread = GetInitValueOfStorageForTaskThread();
                }
                return storageForTaskThread.Value;
            }
        }
        protected virtual ThreadLocal<SimpleSessionStorage> GetInitValueOfStorageForTaskThread()
        {
            return new ThreadLocal<SimpleSessionStorage>(() => new SimpleSessionStorage());
        }
        protected virtual SimpleSessionStorage GetNewStorage()
        {
            return new SimpleSessionStorage();
        }
        public string GetSingleActiveKey()
        {
            var storage = GetSimpleSessionStorage();
            return storage.GetSingleActiveKey();
        }
        public IEnumerable<string> GetAllKeys()
        {
            var storage = GetSimpleSessionStorage();
            return storage.GetAllKeys();
        }
        public static void Application_EndRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (context.Items.ContainsKey(HttpContextSessionStorageKey)) {
                NHibernateSession.CloseAllSessions();
                context.Items.Remove(HttpContextSessionStorageKey);
            }
        }

        public void RemoveSessionForKey(string factoryKey)
        {
            var storage = GetSimpleSessionStorage();
            storage.RemoveSessionForKey(factoryKey);
        }
    }
}