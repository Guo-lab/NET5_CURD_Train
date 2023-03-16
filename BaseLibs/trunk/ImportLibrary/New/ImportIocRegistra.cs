using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ImportLibrary.Validate;
using ProjectBase.CastleWindsor;
using System;
using ProjectBase.Web.Mvc.ValueInFile;

namespace ImportLibrary.New
{
    public class ImportIocRegistra
    {
        public static void IocRegister(IWindsorContainer container, AssemblyFilter allAssembly)
        {
            container.Resolve<DependencyResolver>()
                .RegisterAllImplementations(container, allAssembly, typeof(IValueInFileParser), true);

            container.Register(
                Classes.FromAssemblyInDirectory(allAssembly)
                   .BasedOn(typeof(IImport<>))
                   .WithService.AllInterfaces()
                   .LifestyleTransient(),

                Component.For(typeof(IImportValidateController<>))
                    .ImplementedBy(typeof(ImportValidateController<>))
                    .LifestyleTransient(),

                Classes.FromAssemblyInDirectory(allAssembly)
                   .BasedOn(typeof(IImportValidateHandler<>))
                   .WithService.AllInterfaces());
        }
    }
}
