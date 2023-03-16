using Castle.Windsor;
using ProjectBase.Application;
using ProjectBase.Utils;
using SharpArch.NHibernate;
using System;

namespace ProjectBase.Desktop
{
    /// <summary>
    /// initialize and configure all components, namely nhibernate,,castle windor,log4net,autoMapper
    /// </summary>
    public abstract class BaseDesktopApplication
    {
        private ISessionStorage _sessionStorage;
        protected IAppCommonSetup AppCommonSetup { get; set; }
        protected IAppSpecialSetup AppSpecialSetup { get; set; }

        public IWindsorContainer WindsorContainer { get; set; }
        protected BaseDesktopApplication()
        {
            _sessionStorage = new SimpleSessionStorage();
        }
        
        public virtual void Application_Error(Exception e)
        {
            IUtil util = WindsorContainer.Resolve<IUtil>();
            util.AddLog("Application_Error", e);
        }

        public virtual void Application_Start()
        {
            this.AppCommonSetup.SetupCommonFeature();
            this.AppSpecialSetup.SetupSpecialFeature();
            NHibernateInitializer.Instance().InitializeNHibernateOnce(() => this.AppCommonSetup.InitializeNHibnerate(_sessionStorage));
        }

        public virtual void Application_End(Object sender,EventArgs e)
        {
            NHibernateSession.CloseAllSessions();
            WindsorContainer.Dispose();
        }
    }
}