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
        public static int Application_BeginRequest_Order = -1;
        public static int Application_EndRequest_Order = 99999;

        //<httpcontext�����������첽�̣߳������̳߳أ���� static ���̱߳�����ע������Ϊÿ��������첽��������һ��ֵ��������BeginUsageOutofRequestContext��Application_BeginRequest
        //����ʹ���̳߳أ�http���߳̿������첽�߳�ʹ��ͬһ�̣߳�sessionIsUsedOutofRequestContext�������ֵ�ǰ�߳���;��http�������첽����
        private static ThreadLocal<bool> sessionIsUsedOutofRequestContext = new ThreadLocal<bool>(() => false);
        //��http���̣߳�ISessionStorage����HttpContext����첽�̣߳�����storageForTaskThread�����С�
        private static ThreadLocal<SimpleSessionStorage> storageForTaskThread = new ThreadLocal<SimpleSessionStorage>(() => null);
        //>
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
                registra.AddBeginHandler(Application_BeginRequest, Application_BeginRequest_Order);
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
            if (!sessionIsUsedOutofRequestContext.Value)// && context != null && !context.Items.ContainsKey(KEY_SESSION_STORAGE_ABANDONED))
            { //��request��request��ȡ. �����첽�̵߳�ʱ��������request����֮ǰ����ʱ�����ܷ��ʵ�HttpContext
              //�����첽�߳������̲߳�������context��context.Items���������(�첽�߳̿���ǰ�ŷ��ʵ�context����Ҳ���items�еı��)�����Բ���ʹ��context��context.Items�������̡߳�
              //ʹ���̱߳���sessionIsUsedOutofRequestContext��Ϊ��ǣ��ﵽ�˴�ֻ��http���̲߳Ż�����Ч��
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
                if (storageForTaskThread.Value == null)
                {
                    storageForTaskThread.Value = GetNewStorage();
                }
                return storageForTaskThread.Value;
            }
        }
        //protected virtual ThreadLocal<SimpleSessionStorage> GetInitValueOfStorageForTaskThread()
        //{
        //    return new ThreadLocal<SimpleSessionStorage>(() => new SimpleSessionStorage());
        //}
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
        public void BeginUsageOutofRequestContext()
        {
            //<ÿ�첽�����߳������̱߳���ֵ
            sessionIsUsedOutofRequestContext.Value = true;
            storageForTaskThread.Value = null;
            //>
        }
        public static void Application_BeginRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            //<ÿ���������̱߳���ֵ
            sessionIsUsedOutofRequestContext.Value = false;
            storageForTaskThread.Value = null;
            //>
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