using System;
using System.Collections.Generic;
using System.Threading;
using global::NHibernate;
using ProjectBase.Web.Mvc;

namespace SharpArch.NHibernate.Web.Mvc
{
    /**
     * 在web应用中保存hibernate session，实际就是把SimpleSessionStorage再进一步关联到request的attribute，使得整个request期间都可用。
     * 一般保存在request中，request开始时分配空间，在需要时打开session，request结束时关闭session并删除空间。
     * 但特殊情况下，如应用启动时没有request但可能需要hibernate session，这时就不进行相应的request操作，但而将simple session storage存到一个静态地址中。
     * 这种情况下客户代码需自行调用HibernateSession.CloseAllSessions()以关闭session。
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
            { //有request从request里取
                var storage = (SimpleSessionStorage)context.Items[HttpContextSessionStorageKey];
                if (storage == null)
                {
                    storage = GetNewStorage();
                    context.Items[HttpContextSessionStorageKey] = storage;
                }
                return storage;
            }
            else
            {//没有request就把sessionstorage存在当前线程里
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