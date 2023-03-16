using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectBase.Application;
using ProjectBase.BusinessDelegate;
using ProjectBase.Data;
using ProjectBase.Desktop;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.NHibernate;
using SuperSocket;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SocketBase
{
    /// <summary>
    /// initialize and configure all components, namely nhibernate,castle windor,log4net
    /// </summary>
    public abstract class BaseSocketServerApplication
    {
        public IConfiguration Config { get; }
        public IAppSpecialSetup AppSpecialSetup { get; set; }
        public IAppCommonSetup AppCommonSetup { get; set; }
        public IWindsorContainer WindsorContainer { get; set; }
        public IApplicationBuilder App { get; set; }
        public IWebHostEnvironment Env { get; set; }

        public BaseSocketServerApplication(IConfiguration config)
        {
            Config = config;
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);
            AppCommonSetup.ConfigureServices(services);
            AppSpecialSetup.ConfigureServices(services);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLeftTime, IWindsorContainer container)
        {
            App = app;
            Env = env;
            WindsorContainer = container;
            CastleContainer.WindsorContainer = container;
            MustRegisterEarly(container);

            FrontController.Instance.Init(WindsorContainer, app.ApplicationServices,Env)
                .UseExceptionHandler((session,ex)=> Socket_Appilcation_Error(session,ex));

            //applicationLeftTime.ApplicationStarted.Register(Application_Start);不使用这句，因为第一个请求会触发一次此事件，但触发时间与ConfigureServices+Configure不同步。
            Application_Start();//直接在此处调用Application_Start，而第一个请求的相应处理本身与ConfigureServices+Configure同步，即会在Configure后处理请求
            applicationLeftTime.ApplicationStopping.Register(Application_End);
            //ConfigPathBase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //SignalR请求处理异常未被FrontController截获的在这里处理
            app.UseExceptionHandler((errorApp) => errorApp.Run((context) => SignalR_Application_Error(context)));
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                MapHub(endpoints);
            });
        }
        protected virtual void MapHub(IEndpointRouteBuilder endpoints)
        {
        }

        protected virtual void MustRegisterEarly(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(IUtil))
                    .ImplementedBy(typeof(Util)),
 
                Component.For(typeof(IUtilQuery))
                    .ImplementedBy(typeof(UtilQuery)),

                Component.For(typeof(IApplicationStorage))
                        .ImplementedBy(typeof(SimpleApplicationStorage)),

                Component.For(typeof(RequestListenerRegistra))
                    .ImplementedBy(typeof(RequestListenerRegistra))
             );
        }

        public virtual Task SignalR_Application_Error(HttpContext context)
        {
            var requestContext = WindsorContainer.Resolve<ISocketContextAccessor>().Request;
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var err = exceptionHandlerFeature.Error;
            string msg;
            if (requestContext!=null && requestContext.Items.ContainsKey(BaseControllerExceptionFilter.KEY_EXCEPTION_LOGGED))
            {
                msg = (requestContext.Items[BaseControllerExceptionFilter.KEY_EXCEPTION_LOGGED]?.ToString())??"";
            }
            else
            {
                var allMsg = "";
                RecursiveMessage(err, ref allMsg);
                msg = allMsg;
                IUtil util = WindsorContainer.Resolve<IUtil>();
                util.AddLog("SignalR_Application_Error", err);
            }
            context.Response.ContentType = "application/json";
            if (!Env.IsDevelopment())
            {
                msg= "系统运行错误：" + err.GetType();
                context.Response.Clear();
                return context.Response.WriteAsync(msg);
            }
            return context.Response.WriteAsync(msg + "\r\n" + err.Source + "\r\n" + err.StackTrace);
        }

        /// <summary>
        /// Socket请求处理异常未被FrontController截获的在这里处理.即，此处处理的异常是未经MC框架处理的异常。
        /// </summary>
        /// <param name="session"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual ValueTask<bool> Socket_Appilcation_Error(IAppSession session, PackageHandlingException<TextPackageInfo> ex)
        {
            string msg = GetSocketAppilcationError(ex);
            if (!IsMessageLoggedByBaseController()) 
            { 
                IUtil util = WindsorContainer.Resolve<IUtil>();
                util.AddLog("Socket_Application_Error", ex);
            }
            Console.Write(msg);
            return ValueTask.FromResult(false);
        }
        protected string GetSocketAppilcationError(PackageHandlingException<TextPackageInfo> ex)
        {
            var requestContext = WindsorContainer.Resolve<ISocketContextAccessor>().Request;
            if (IsMessageLoggedByBaseController())
            {
                return (requestContext!.Items[BaseControllerExceptionFilter.KEY_EXCEPTION_LOGGED]?.ToString()) ?? "";
            }
            else
            {
                var allMsg = "";
                RecursiveMessage(ex, ref allMsg);
                return allMsg;
            }
        }

        protected bool IsMessageLoggedByBaseController()
        {
            var requestContext = WindsorContainer.Resolve<ISocketContextAccessor>().Request;
            return requestContext != null && requestContext.Items.ContainsKey(BaseControllerExceptionFilter.KEY_EXCEPTION_LOGGED);
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

        public virtual void Application_Start()
        {
            AppCommonSetup.Env = Env;
            AppCommonSetup.WindsorContainer = WindsorContainer;
            AppCommonSetup.SetupCommonFeature();
            AppSpecialSetup.Env = Env;
            AppSpecialSetup.WindsorContainer = WindsorContainer;
            AppSpecialSetup.SetupSpecialFeature();

            var sessionStorage = WindsorContainer.Resolve<ISessionStorage>();
            NHibernateInitializer.Instance().InitializeNHibernateOnce(() => this.AppCommonSetup.InitializeNHibnerate(sessionStorage));

            //TODO:适当时机应启用 CheckIoc();
        }

        public virtual void Application_End()
        {
            WindsorContainer.Dispose();
        }

        private string GetRunningPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\" + relativePath;
        }

    }
}
