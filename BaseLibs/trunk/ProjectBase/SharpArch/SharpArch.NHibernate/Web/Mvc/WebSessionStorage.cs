using System;
using System.Collections.Generic;
using System.Threading;
using global::NHibernate;
using ProjectBase.BusinessDelegate;
using ProjectBase.Web.Mvc;

namespace SharpArch.NHibernate.Web.Mvc
{
    /**
     * 在web应用中保存hibernate session，实际就是把SimpleSessionStorage再进一步关联到request的attribute，使得整个request期间都可用。
     * 一般保存在request中，在需要时打开session（无事务的在执行第一个查询时打开，有事务标记的在事务开始时打开），request结束时关闭session并删除空间。
     * 但特殊情况下，如应用启动时没有request但可能需要hibernate session，这时就不进行相应的request操作，但而将simple session storage存到一个静态地址中。
     * 这种情况下客户代码需自行调用HibernateSession.CloseAllSessions()以关闭session。
     * @see --advanced
     */
    public class WebSessionStorage : ISessionStorage
    {
        protected const string HttpContextSessionStorageKey = "HttpContextSessionStorageKey";
        public static readonly string KEY_SESSION_STORAGE_ABANDONED = "SessionStorageInThisThreadWasAbandoned";
        public static int Application_BeginRequest_Order = -1;
        public static int Application_EndRequest_Order = 99999;

        //<httpcontext及其启动的异步线程，共用线程池，因此 static 的线程变量需注意自行为每个请求和异步任务重置一次值。见下面BeginUsageOutofRequestContext和Application_BeginRequest
        //由于使用线程池，http主线程可能与异步线程使用同一线程，sessionIsUsedOutofRequestContext用于区分当前线程用途是http请求还是异步任务
        private static ThreadLocal<bool> sessionIsUsedOutofRequestContext = new ThreadLocal<bool>(() => false);
        //对http主线程，ISessionStorage存在HttpContext里，对异步线程，存在storageForTaskThread变量中。
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
            { //有request从request里取. 由于异步线程的时机可能在request结束之前，此时还是能访问到HttpContext
              //由于异步线程与主线程并发访问context和context.Items会产生问题(异步线程可能前脚访问到context后脚找不到items中的标记)，所以不能使用context和context.Items来区别线程。
              //使用线程变量sessionIsUsedOutofRequestContext作为标记，达到此处只有http主线程才会进入的效果
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
            //<每异步任务线程重置线程变量值
            sessionIsUsedOutofRequestContext.Value = true;
            storageForTaskThread.Value = null;
            //>
        }
        public static void Application_BeginRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            //<每请求重置线程变量值
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