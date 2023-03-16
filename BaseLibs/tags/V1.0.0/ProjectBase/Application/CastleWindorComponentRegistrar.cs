using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ProjectBase.BusinessDelegate;
using ProjectBase.Command;
using ProjectBase.Data;
using ProjectBase.Domain;
using ProjectBase.Domain.Transaction;
using ProjectBase.DomainEvent;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using System;
namespace ProjectBase.Application
{
    public class CastleWindorComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericDaosTo(container);
            //AddCustomDaosTo(container);
            AddQueryObjectsTo(container);
            AddDomainEvent(container);
            AddCommand(container);
            AddBusinessDelegateTo(container);
            AddOtherTo(container);
        }

        private static string GetAssemblyPath()
        {
            return AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
        }
        private static void AddDomainEvent(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(IDispatcher))
                    .ImplementedBy(typeof(Dispatcher)));

            container.Register(
               Classes.FromAssemblyInDirectory(new AssemblyFilter(GetAssemblyPath()))
                   .BasedOn(typeof(IDomainEventHandler<>))
                   .WithService.AllInterfaces());
        }
        private static void AddCommand(IWindsorContainer container)
        {
            AssemblyFilter allAssembly = new AssemblyFilter(GetAssemblyPath());

            container.Register(
                Classes
                   .FromAssemblyInDirectory(allAssembly)
                   .BasedOn(typeof(ICommandHandler<,>))
                   .WithService.AllInterfaces());
            container.Register(
                Classes
                   .FromAssemblyInDirectory(allAssembly)
                   .BasedOn(typeof(ICommandHandler<>))
                   .WithService.AllInterfaces());

            //container.Register(
            //    Component.For(typeof(ICommandSender<,>))
            //        .ImplementedBy(typeof(CommandSender<,>)));
            //container.Register(
            //   Classes.FromAssemblyInDirectory(new AssemblyFilter(GetAssemblyPath()))
            //       .BasedOn(typeof(ICommandHandler<,>))
            //       .WithService.FirstInterface());
        }
        private static void AddBusinessDelegateTo(IWindsorContainer container)
        {
            //container.Register(
            //    AllTypes
            //        .FromAssemblyNamed(ProjectHierarchy.BusinessDelegateNS)
            //        .BasedOn<IBusinessDelegate>()
            //        .WithService.AllInterfaces());//.FirstNonGenericCoreInterface(ProjectHierarchy.DomainNS));
            AssemblyFilter allAssembly = new AssemblyFilter(GetAssemblyPath());
            container.Register(
                 Classes.FromAssemblyInDirectory(allAssembly)
                 .BasedOn(typeof(ICommonFilter<,>))
                 .WithService.AllInterfaces()
                .LifestyleTransient());


            container.Register(
                 Classes.FromAssemblyInDirectory(allAssembly)
                 .BasedOn(typeof(ICommonReader<,,>))
                 .WithService.DefaultInterfaces()
                .LifestyleTransient());

            container.Register(
                 Classes.FromAssemblyInDirectory(allAssembly)
                 .BasedOn(typeof(IBusinessDelegate))
                 .WithService.DefaultInterfaces());

            container.Register(
                  Component.For(typeof(IValidateController<,>))
                  .ImplementedBy(typeof(ValidateController<,>))
                  .LifestyleTransient());
            container.Register(
                Classes
                   .FromAssemblyInDirectory(allAssembly)
                   .BasedOn(typeof(IValidator<,>))
                   .WithService.AllInterfaces());

            //keep this as the last one BD registeration,so derived class can be used first
            //container.Register(
            //    Component.For(typeof(ICommonBD<,>))
            //        .ImplementedBy(typeof(CommonBD<,>)));

            //container.Register(
            //    Component.For(typeof(ICommandProcessor))
            //        .ImplementedBy(typeof(CommandProcessor))
            //        .Named("commandProcessor"));

            //container.Register(
            //    AllTypes.FromAssemblyNamed(ProjectHierarchy.BusinessDelegateNS)
            //        .BasedOn(typeof(ICommandHandler<>))
            //        .WithService.FirstInterface());

        }

        //private static void AddCustomDaosTo(IWindsorContainer container)
        //{
        //    container.Register(
        //        AllTypes
        //            .FromAssemblyNamed(ProjectHierarchy.DataNS)
        //            .BasedOn(typeof(IGenericDaoWithTypedId<,>))
        //            .WithService.FirstNonGenericCoreInterface(ProjectHierarchy.DomainNS));
        //}

        public static void AddGenericDaosTo(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(IEntityDuplicateChecker))
                    .ImplementedBy(typeof(EntityDuplicateChecker))
                    .Named("entityDuplicateChecker"));

            container.Register(
                Component.For(typeof(IGenericDao<>))
                    .ImplementedBy(typeof(BaseNHibernateLinqDaoWithTypedId<,>))
                    .Forward(typeof(INHibernateRepository<>))
                    .Forward(typeof(ILinqRepository<>))
                    .Forward(typeof(IRepository<>))
                    .Named("nhibernateRepositoryType1"));
            container.Register(
                Component.For(typeof(IGenericDaoWithAssignedIntId<>))
                    .ImplementedBy(typeof(BaseNHibernateLinqDaoWithTypedId<,>))
                    .Forward(typeof(INHibernateRepository<>))
                    .Forward(typeof(ILinqRepository<>))
                    .Forward(typeof(IRepository<>))
                    .Named("nhibernateRepositoryType2"));

            container.Register(
                Component.For(typeof(IGenericDaoWithAssignedGuid<>))
                    .ImplementedBy(typeof(BaseNHibernateLinqDaoWithTypedId<,>))
                    .Forward(typeof(INHibernateRepository<>))
                    .Forward(typeof(ILinqRepository<>))
                    .Forward(typeof(IRepository<>))
                    .Named("nhibernateRepositoryType3"));

            container.Register(
                Component.For(typeof(IGenericDaoWithTypedId<,>))
                    .ImplementedBy(typeof(BaseNHibernateLinqDaoWithTypedId<,>))
                    .Forward(typeof(INHibernateRepositoryWithTypedId<,>))
                    .Forward(typeof(ILinqRepositoryWithTypedId<,>))
                    .Forward(typeof(IRepositoryWithTypedId<,>))
                    .Named("nhibernateRepositoryWithTypedId"));

        }

        private static void AddQueryObjectsTo(IWindsorContainer container)
        {
            //container.Register(
            //    AllTypes.FromAssemblyNamed(ProjectHierarchy.ControllerNS)
            //        .BasedOn<NHibernateQuery>()
            //        .WithService.FirstInterface());
            //container.Register(
            //    Component.For(typeof(IUtilQuery))
            //        .ImplementedBy(typeof(UtilQuery)));
        }

        private static void AddOtherTo(IWindsorContainer container)
        {
            container.Register(
               Component.For(typeof(IExceptionTranslator))
                   .ImplementedBy(typeof(NHibernateExceptionTranslator)));
            container.Register(
                Component.For(typeof(ITransactionHelper))
                    .ImplementedBy(typeof(TransactionHelper)));
            container.Register(
                Component.For(typeof(IDbContext))
                    .ImplementedBy(typeof(DbContext)));
        }
    }
}
