using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ESC5.AppBase.AppShutdown;
using ProjectBase.BusinessDelegate;
using ProjectBase.CastleWindsor;
using ProjectBase.Utils;

namespace ESC5.AppBase.LTT
{
    public class LongTimeTaskManager: ILongTimeTaskManager, IAppShutdownConfirmer
    {
        private static readonly string BIZ_EXCEPTION_LTT_ADD_FAILED= "LongTimeTask_AddFailed";
        private static readonly string BIZ_EXCEPTION_LTT_LOCKED_FOR_SHUTDOWN = "LongTimeTask_LockedForShutdown";

        public IUtil Util { get; set; }
        
        private IDictionary<string, LongTimeTaskTrace> traceMap = new Dictionary<string, LongTimeTaskTrace>();

        private bool lockedForAppShutdown = false;

        private static object signal = new();
        public void Start(LongTimeTaskTrace trace, ILongTimeTask task, Action afterAction)
        {
            var traceKey = task.GetTraceKey(trace);
            lock (signal)
            {
                if (lockedForAppShutdown) throw new BizException(BIZ_EXCEPTION_LTT_LOCKED_FOR_SHUTDOWN);
                trace.TraceKey = traceKey;
                var ok = traceMap.TryAdd(traceKey, trace);
                if (!ok) throw new BizException(BIZ_EXCEPTION_LTT_ADD_FAILED);
            }
            Task.Run(() =>
            {
                task.Execute(traceKey);
                afterAction();
                Finish(traceKey);
            });
        }
        private void Finish(string traceKey)
        {
            lock (signal)
            {
                traceMap.Remove(traceKey);
            }
        }
        public void UpdateProgress(string traceKey, int num,string info)
        {
            lock (signal)
            {
                traceMap[traceKey].ProgressNum = num;
                traceMap[traceKey].ProgressInfo = info;
                Util.AddLog(info);
            }
        }
        public bool IsLocked(string traceKey)
        {
            lock (signal)
            {
                return lockedForAppShutdown || traceMap.ContainsKey(traceKey);
            }
        }
        public IList<LongTimeTaskTrace> GetTraceList()
        {
            return traceMap.Values.ToList();
        }
        public IList<LongTimeTaskTrace> GetTraceList(int userId)
        {
            return traceMap.Values.Where(o=>o.User==userId || o.Subscribers.Contains(userId)).ToList();
        }
        public bool IsReadyToShutdown()
        {
            lock (signal)
            {
                return traceMap.Count==0;
            }
        }

        public void OnShuttingDown()
        {
            lock (signal)
            {
                lockedForAppShutdown = true;
            }
        }

        public static void IocRegister(IWindsorContainer container,AssemblyFilter allAssembly)
        {
            container.Register(
                Component.For<ILongTimeTaskManager>().ImplementedBy<LongTimeTaskManager>(),
                Component.For<IAppShutdownConfirmer>().ImplementedBy<LongTimeTaskManager>().Named("LongTimeTaskManagerAsIAppShutdownConfirmer")
            );
            container.Resolve<DependencyResolver>()
                .RegisterAllImplementations(container, allAssembly, typeof(ILongTimeTask));
        }
    }

}

