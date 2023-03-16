using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESC5.AppBase.AppShutdown
{
     public class AppShutdownManager : IAppShutdownManager
    {
        public IWindsorContainer WindsorContainer { get; set; }
        public void NotifyShutdown()
        {
            var confirmers = WindsorContainer.ResolveAll<IAppShutdownConfirmer>();
            foreach (var confirmer in confirmers)
            {
                confirmer.OnShuttingDown();
            }
        }
        public bool IsReadyToShutdown()
        {
            var confirmers = WindsorContainer.ResolveAll<IAppShutdownConfirmer>();
            return confirmers.All(o => o.IsReadyToShutdown());
        }

        public static void IocRegister(IWindsorContainer container, AssemblyFilter allAssembly)
        {
            container.Register(
                Component.For<IAppShutdownManager>().ImplementedBy<AppShutdownManager>(),
                Classes.FromAssemblyInDirectory(allAssembly).BasedOn<IAppShutdownConfirmer>().WithServiceBase()
            );
        }
    }
}
