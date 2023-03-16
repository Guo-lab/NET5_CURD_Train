using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using SharpArch.Domain;
using SharpArch.NHibernate;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketBase
{
    /// <summary>
    /// Socket请求统一入口
    /// </summary>
    public class FrontController
    {
        public static string DEFAULT_AREA = "DefaultArea";

        //此处自己创建单例,再到后面注册到container
        private static FrontController singleton = new FrontController();
        public static FrontController Instance
        {
            get
            {
                return singleton;
            }
        }

        public IWindsorContainer WindsorContainer { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public IHostEnvironment Env { get; set; }

        /// <summary>
        ///  每连接一个或多个线程,每线程同时只会有一个Request(socket或signalR请求)，即当前Request
        /// </summary>
        public static ThreadLocal<RequestContext?> RequestForActionThread { get; set; }
        public static ThreadLocal<object> SerialLock = new ThreadLocal<object>(()=>new object());//顺序同步锁,用于顺序处理每个Request(socket或signalR请求)

        public Func<IAppSession, PackageHandlingException<TextPackageInfo>,ValueTask<bool>> ExceptionHandler { get; private set; }

        private IList<IActionFilter> globalActionFilters = new List<IActionFilter>();
        private IList<IActionFilter> GlobalActionFilters
        {
            get
            {
                if (globalActionFilters.Count == 0)
                {
                    globalActionFilters = WindsorContainer.ResolveAll<IActionFilter>().ToList();
                }
                return globalActionFilters;
            }
        }
        public FrontController Init(IWindsorContainer iocContainer, IServiceProvider serviceProvider, IHostEnvironment env)
        {
            WindsorContainer = iocContainer;
            ServiceProvider = serviceProvider;
            Env = env;
            WindsorContainer.Register(Component.For<FrontController>().Instance(Instance));
            return this;
        }
        public void UseExceptionHandler(Func<IAppSession, PackageHandlingException<TextPackageInfo>, ValueTask<bool>> handler)
        {
            ExceptionHandler = handler;
        }
        /// <summary>
        /// 注册全局过滤器类
        /// </summary>
        /// <typeparam name="TActionFilter"></typeparam>
        public void AddActionFilter<TActionFilter>()
        {
            WindsorContainer.Register(Component.For<IActionFilter>().ImplementedBy(typeof(TActionFilter)));
        }
        public static void RegisterRequestMapper<TRequestMapper>()
        {
            Instance.WindsorContainer.Register(Component.For<IRequestMapper>().ImplementedBy(typeof(TRequestMapper)));
        }

        public async ValueTask HandlePackageAsync(IAppSession session, TextPackageInfo package)
        {
            RcResult rcResult;
            lock (SerialLock)
            {//顺序处理一个连接的多个请求。一个连接的多个请求可能属同一线程也可能不同。加锁防止不同线程时多请求冲突，使得按顺序处理

                var mapper = WindsorContainer.Resolve<IRequestMapper>();
                var requestContext = mapper.MapSocket(package);
                requestContext.RequestSource = package;
                requestContext.Session = session;
                requestContext.RequestServices = ServiceProvider;
                RequestForActionThread = new ThreadLocal<RequestContext?>(() => requestContext);

                var registra = WindsorContainer.Resolve<RequestListenerRegistra>();
                foreach (var handler in registra.BeginRequest.Values)
                {
                    handler.Invoke(requestContext);
                }
                rcResult = HandleSocketRequest(session, requestContext);
                foreach (var handler in registra.EndRequest.Values)
                {
                    handler.Invoke(requestContext);
                }

                RequestForActionThread.Value = null;
            }
            await ExecuteResultForSocketClientAsync(session, rcResult);
        }

        private RcResult HandleSocketRequest(IAppSession session, SocketRequestContext requestContext)
        {
            var controllerInstance = ResolveController<BaseSocketController>(requestContext);
            ResolveAction(requestContext);
            controllerInstance.Request = requestContext;

            IActionResult? result;
            try
            {
                result=InvokeAction(requestContext, controllerInstance);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException!;
                }
                result = HandleException(requestContext, e,ControllerKindEnum.Socket);
                if (result == null)
                {
                    result = new RichClientJsonResult(false, RichClientJsonResult.Command_Exception, e.Message);
                }
            }
            return (RcResult)result.ExecuteResult()!;
        }
        private async ValueTask ExecuteResultForSocketClientAsync(IAppSession session,RcResult rcResult)
        {
            var connectionManager = WindsorContainer.Resolve<IConnectionManager>();
            //if (rcResult.command == RichClientJsonResult.Command_Exception)
            if (rcResult.command != RichClientJsonResult.Command_Noop)
            {
                await connectionManager.SendToSocketAsync(session, rcResult);
            }
            else
            {
                await ValueTask.CompletedTask;
            }
        }
        private TController ResolveController<TController>(RequestContext context) where TController : BaseController
        {
            var controllerIocKey = context.Area + "." + context.Controller + "Controller";
            object? controllerInstance = WindsorContainer.Resolve<IController>(controllerIocKey);
            if (controllerInstance == null) throw new NetArchException("未找到对应的Controller：" + controllerIocKey);

            context.ControllerClass = controllerInstance.GetType();
            if (!context.ControllerClass.IsAssignableTo(typeof(TController))) throw new NetArchException("找到对应的Controller：" + controllerIocKey + ",但类型不符");

            context.ControllerInstance = (TController)controllerInstance;
            return (TController)controllerInstance;
        }
        private void ResolveAction(RequestContext context)
        {
            var actionMethod = context.ControllerClass.GetMethod(context.Action);//action名不可重复
            if (actionMethod == null) throw new NetArchException("未找到对应的方法：" + context.ControllerClass.FullName + "." + context.Action);
            context.ActionMethod = actionMethod;
        }
        private IActionResult InvokeAction(RequestContext requestContext, object controllerInstance)
        {
            var context = new ActionFilterContext()
            {
                Request = requestContext
            };

            ExecuteFilters(GlobalActionFilters, context, true);
            if (context.Result != null) return context.Result;

            var filters = requestContext.ActionMethod.CustomAttributes
                .Where(o => o.AttributeType.IsAssignableTo(typeof(ActionFilterAttribute)))
                .Select(o => (IActionFilter)requestContext.ActionMethod.GetCustomAttributes(o.AttributeType).First());//热编译后方法上会认为有两个transaction标记实例,因此此处保证只取一个

            ExecuteFilters(filters, context, true);
            if (context.Result != null) return context.Result;

            try
            {
                var result = (IActionResult?)requestContext.ActionMethod.Invoke(controllerInstance, requestContext.Parameters);
                if (result == null) throw new NetArchException("Action 不可返回null");
                context.Result = result;
            }
            catch (TargetInvocationException e)
            {
                context.Exception = e.InnerException;
                context.ExceptionHandled = false;
            }
            catch (Exception e)
            {
                context.Exception = e;
                context.ExceptionHandled = false;
            }

            ExecuteFilters(filters, context, false);
            ExecuteFilters(GlobalActionFilters, context, false);
            if (context.Exception != null && !context.ExceptionHandled) throw new ActionInvocationException(context.Request.ACA, context.Exception);

            Check.Ensure(context.Result != null, "OnActionExecuted不可将Result设置为空值");
            return context.Result!;
        }
        private void ExecuteFilters(IEnumerable<IActionFilter> filters, ActionFilterContext context, bool before)
        {
            if (filters == null) return;
            if (before)
            {
                foreach (var filter in filters.OrderBy(o => o.Order))
                {
                    if (filter.ControllerKind == ControllerKindEnum.Hub && context.Request is SocketRequestContext
                        || filter.ControllerKind == ControllerKindEnum.Socket && context.Request is SignalRRequestContext) continue;//种类不匹配就不执行

                    filter.OnActionExecuting(context);
                    if (context.Result != null) return;
                }
            }
            else
            {
                foreach (var filter in filters.OrderByDescending(o => o.Order))
                {
                    if (filter.ControllerKind == ControllerKindEnum.Hub && context.Request is SocketRequestContext
                        || filter.ControllerKind == ControllerKindEnum.Socket && context.Request is SignalRRequestContext) continue;//种类不匹配就不执行

                    filter.OnActionExecuted(context);
                }
            }
        }
        private void RecursiveMessage(Exception e, ref string allMsg)
        {
            allMsg += e.Message;
            if (e.InnerException != null)
            {
                allMsg += "\r\n";
                RecursiveMessage(e.InnerException, ref allMsg);
            }
        }
        private IActionResult? HandleException(RequestContext request, Exception ex,ControllerKindEnum kind)
        {
            var filters = WindsorContainer.ResolveAll<IExceptionFilter>().Where(o=>o.ControllerKind==kind).OrderBy(o=>o.Order);
            foreach (var filter in filters)
            {
                var context = new ExceptionContext()
                {
                    RequestContext = request,
                    Exception = ex,
                };

                filter.OnException(context);
                if (context.ExceptionHandled) return context.Result;
            }
            return null;
        }

        public object? HandleSignalRRequest(HubCallerContext context, SignalRRequestParam param)
        {
            object? rtn;
            lock (SerialLock)
            {//顺序处理一个连接的多个请求。一个连接的多个请求可能属同一线程也可能不同。加锁防止不同线程时多请求冲突，使得按顺序处理
                var mapper = WindsorContainer.Resolve<IRequestMapper>();
                var requestContext = mapper.MapSingalR(param);
                requestContext.HubCallerContext = context;
                requestContext.RequestServices = ServiceProvider;
                RequestForActionThread = new ThreadLocal<RequestContext?>(() => requestContext);

                var registra = WindsorContainer.Resolve<RequestListenerRegistra>();
                foreach (var handler in registra.BeginRequest.Values)
                {
                    handler.Invoke(requestContext);
                }
                rtn = HandleSignalRRequest(requestContext, context, param);
                foreach (var handler in registra.EndRequest.Values)
                {
                    handler.Invoke(requestContext);
                }
                RequestForActionThread.Value = null;
            }
            return rtn;
        }
        private object? HandleSignalRRequest(SignalRRequestContext requestContext, HubCallerContext context, SignalRRequestParam param)
        {
            var controllerInstance = ResolveController<BaseSignalRController>(requestContext);
            ResolveAction(requestContext);

            var paramMap = param.ActionParamJsonMap;
            var parameters = new List<object?>();
            var parameterTypes = new List<Type>();
            foreach (var p in requestContext.ActionMethod.GetParameters())
            {
                if (p.Name == null) throw new NetArchException("Action不可有匿名参数");
                var pvalueJsonStr = paramMap.SingleOrDefault(pm => pm.Key.Equals(p.Name, StringComparison.OrdinalIgnoreCase)).Value;
                if (pvalueJsonStr != default)
                {
                    var pvalue = JsonConvert.DeserializeObject(pvalueJsonStr, p.ParameterType);
                    parameters.Add(pvalue);
                }
                else
                {
                    parameters.Add(null);
                }
                parameterTypes.Add(p.ParameterType);
            }
            if (parameters.Count > 0)
            {
                requestContext.Parameters = parameters.ToArray();
                requestContext.ParameterTypes = parameterTypes.ToArray();
            }
            requestContext.RequestSource = context.GetHttpContext().Request;
            controllerInstance.Request = requestContext;

            try
            {
                var result = InvokeAction(requestContext, controllerInstance);
                return result.ExecuteResult();
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException!;
                }
                var rtn = HandleException(requestContext, e,ControllerKindEnum.Hub);
                if (rtn == null) return new RichClientJsonResult(false, RichClientJsonResult.Command_Exception, e.Message).ExecuteResult();
                return rtn.ExecuteResult();
            }
        }
    }

    public class ActionInvocationException : Exception
    {
        public ActionInvocationException(string msg,Exception ex):base("Action执行异常："+ msg,ex)
        {

        }
    }
}
