using System;
using System.Collections.Generic;
using System.Threading;
using global::NHibernate;
using ProjectBase.BusinessDelegate;
using ProjectBase.Web.Mvc;
using SharpArch.NHibernate;

namespace SocketBase
{
    /**
     * 在socket应用中保存hibernate session，实际就是把SimpleSessionStorage再进一步关联到socket的一次通讯(request)，使得整个request期间都可用。
     * 对于建立了MC结构的socket应用，一次通讯对应一个Action执行，因此将hb session保存在SocketBase.Request中，request开始时分配空间，在需要时打开session，action结束时关闭session并删除空间。
     * 但特殊情况下，如应用启动时没有request但可能需要hibernate session，这时就不进行相应的request操作，但而将simple session storage存到一个静态地址中。
     * 这种情况下客户代码需自行调用HibernateSession.CloseAllSessions()以关闭session。
     * @see --advanced
     */
    public class SocketSessionStorage : ISessionStorage
    {
        protected const string HttpContextSessionStorageKey = "HttpContextSessionStorageKey";
        public static readonly string KEY_SESSION_STORAGE_ABANDONED = "SessionStorageInThisThreadWasAbandoned";
        public static int Application_EndRequest_Order = 99999;

        private static ThreadLocal<SimpleSessionStorage> storageForTaskThread;

        public ControllerKindEnum ControllerKind=> ControllerKindEnum.All;

        public ISocketContextAccessor Hca { get; set; }

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
            var context = Hca.Request;
            if (context != null && !context.Items.ContainsKey(KEY_SESSION_STORAGE_ABANDONED))
            { //有request从request里取. 由于异步线程的时机可能在request结束之前，此时可能是与request同一线程还是能访问到HttpContext，所以需要异步线程启动时给个标志，此处通过这个标志判断异步任务不使用HttpContext
                object? storage=null;
                context.Items.TryGetValue(HttpContextSessionStorageKey, out storage);
                if (storage == null)
                {
                    storage = GetNewStorage();
                    context.Items[HttpContextSessionStorageKey] = storage;
                }
                return (SimpleSessionStorage)storage;
            }
            else
            {//没有request就把sessionstorage存在当前线程里
                if (storageForTaskThread == null)
                {
                    storageForTaskThread = GetInitValueOfStorageForTaskThread();
                }
                return storageForTaskThread.Value!;
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

        public static void Application_EndRequest(RequestContext context)
        {
            if (context.Items.ContainsKey(HttpContextSessionStorageKey))
            {
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
