using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectBase.Application;
using ProjectBase.BusinessDelegate;
using ProjectBase.Data;
using ProjectBase.Domain;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc.Tool;
using SharpArch.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc
{
    /// <summary>
    /// initialize and configure all components, namely nhibernate,mvc3,castle windor,log4net,spark,autoMapper
    /// </summary>
    public abstract class BaseMvcApplication
    {
        public IConfiguration Config { get; }
        public IAppSpecialSetup AppSpecialSetup { get; set; }
        public IAppCommonSetup AppCommonSetup { get; set; }
        public IWindsorContainer WindsorContainer { get; set; }
        public IApplicationBuilder App { get; set; }
        public IWebHostEnvironment Env { get; set; }

        public BaseMvcApplication(IConfiguration config)
        {
            Config = config;
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);
            AppCommonSetup.ConfigureServices(services);
            AppSpecialSetup.ConfigureServices(services);
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLeftTime, IWindsorContainer container)
        {
            App = app;
            Env = env;
            WindsorContainer = container;
            CastleContainer.WindsorContainer = container;
            MustRegisterEarly(container);

            //applicationLeftTime.ApplicationStarted.Register(Application_Start);不使用这句，因为第一个请求会触发一次此事件，但触发时间与ConfigureServices+Configure不同步。
            Application_Start();//直接在此处调用Application_Start，而第一个请求的相应处理本身与ConfigureServices+Configure同步，即会在Configure后处理请求
            applicationLeftTime.ApplicationStopping.Register(Application_End);
            //ConfigPathBase(app);
            app.UseMiddleware<RequestListenerMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseExceptionHandler((errorApp) => errorApp.Run((context) => Application_Error(context)));
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();
            app.UseCors("CustomCorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaarea",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }

        private void ConfigPathBase(IApplicationBuilder app)
        {
            var pathBase = Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");
            // 如果 ASPNETCORE_PATHBASE 的值不为空， 则使用 Pathbase 中间件
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(new PathString(pathBase));
            }
        }

        protected virtual void MustRegisterEarly(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(IUtil))
                    .ImplementedBy(typeof(Util)));
            container.Register(
                Component.For(typeof(IUtilQuery))
                    .ImplementedBy(typeof(UtilQuery)));
            container.Register(
                    Component.For(typeof(IApplicationStorage))
                        .ImplementedBy(typeof(WebApplicationStorage)));
            container.Register(
                Component.For(typeof(RequestListenerRegistra))
                    .ImplementedBy(typeof(RequestListenerRegistra)));

        }

        public virtual Task Application_Error(HttpContext context)
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var err = exceptionHandlerFeature.Error;
            IUtil util = WindsorContainer.Resolve<IUtil>();
            util.AddLog("Application_Error", err);
            if (!Env.IsDevelopment())
            {
                var html = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body><h2>系统运行错误：" + err.GetType() + "</h2></body></htm>";
                context.Response.Clear();
                return context.Response.WriteAsync(html);
            }
            return context.Response.WriteAsync(err.Message + "\r\n" + err.Source + "\r\n" + err.StackTrace);
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

            if (ClientVmMetadataProvider.ShouldRun)
            {
                RunDevTool();
            }
           // CheckIoc();
        }

        public virtual void Application_End()
        {
            WindsorContainer.Dispose();
        }

        private void RunDevTool()
        {
            try
            {
                string[] list = ProjectHierarchy.ViewModelNS.Split(',');
                var types = list.Select(x => Assembly.LoadFrom(GetRunningPath(x + ".dll"))).ToArray().SelectMany(a => a.GetTypes());

                var provider = WindsorContainer.Resolve<IModelMetadataProvider>();
                ClientVmMetadataProvider.CreateAll(provider, types);
                ClientVmDefinitionTool.CreateAll(provider, types);
            }
            catch (Exception e)
            {
                Console.Write("开发工具运行错：" + e.StackTrace);
            }
        }
        private string GetRunningPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\" + relativePath;
        }

        private void CheckIoc()
        {
            if (!Env.IsDevelopment()) return;

            var host = (IDiagnosticsHost)WindsorContainer.Kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
            var diagnostics = host.GetDiagnostic<IPotentiallyMisconfiguredComponentsDiagnostic>();
            var handlers = diagnostics.Inspect();
            var diagnostics2 = host.GetDiagnostic<IDuplicatedDependenciesDiagnostic>();
            var handlers2 = diagnostics2.Inspect();

            if (handlers.Length > 0)
            {
                Console.WriteLine("*******可能循环依赖***********************************************");
                foreach (var handler in handlers){
                    Console.WriteLine(handler.ComponentModel.Name +"--"+ handler.CurrentState.ToString());
                }
                Console.WriteLine("*******可能循环依赖");
                throw new Exception("检测到循环依赖，可通过输出窗口查看");
            }
            if (handlers2.Length > 0) {
                Console.WriteLine("*******重复依赖***********************************************");
                foreach (var handler2 in handlers2)
                {
                    Console.WriteLine(handler2.First.ComponentModel.Name + "");
                    foreach (var h in handler2.Second) {
                        if (h.Dependency1.TargetType == typeof(string) || h.Dependency1.TargetType.IsValueType) continue;
                        Console.WriteLine("               "+h.Dependency1.ToString() + "==" + h.Dependency2.ToString());
                    }
                    Console.WriteLine("--------------------------------------------------------");
                }
                Console.WriteLine("*******重复依赖");
            }
        }
    }
}