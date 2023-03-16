using System;
using System.Collections.Generic;
using System.Threading;
using global::NHibernate;
using ProjectBase.Web.Mvc;

namespace SharpArch.NHibernate.Web.Mvc
{
    /**
     * ��webӦ���б���hibernate session��ʵ�ʾ��ǰ�SimpleSessionStorage�ٽ�һ��������request��attribute��ʹ������request�ڼ䶼���á�
     * һ�㱣����request�У�request��ʼʱ����ռ䣬����Ҫʱ��session��request����ʱ�ر�session��ɾ���ռ䡣
     * ����������£���Ӧ������ʱû��request��������Ҫhibernate session����ʱ�Ͳ�������Ӧ��request������������simple session storage�浽һ����̬��ַ�С�
     * ��������¿ͻ����������е���HibernateSession.CloseAllSessions()�Թر�session��
     * @see --advanced
     */
    public class WebSessionStorage : ISessionStorage
    {
        protected const string HttpContextSessionStorageKey = "HttpContextSessionStorageKey";

        private static ThreadLocal<SimpleSessionStorage> storageForTaskThread;
        public Microsoft.AspNetCore.Http.IHttpContextAccessor Hca { get; set; }

        private RequestListenerRegistra registra;
        public RequestListenerRegistra Registra
        {
            get
            {
                return registra;
            }
            set
            {
                registra = value;
                //registra.AddBeginHandler(Application_BeginRequest);
                registra.AddEndHandler(Application_EndRequest);
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
            if (context != null)
            { //��request��request��ȡ
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

        public static void Application_EndRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            NHibernateSession.CloseAllSessions();
            context.Items.Remove(HttpContextSessionStorageKey);
        }
    }
}