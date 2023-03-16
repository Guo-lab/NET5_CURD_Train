using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ProjectBase.BusinessDelegate;
using ProjectBase.Command;
using ProjectBase.Data;
using ProjectBase.Domain;
using ProjectBase.Domain.Transaction;
using ProjectBase.DomainEvent;
using ProjectBase.Web.Mvc;
using SharpArch.Domain.DomainModel;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProjectBase.Application
{
    public class CastleWindorComponentRegistrar
    {
        public static IList<Type> IncludeCommonBDForDOs { get; set; }
        public static IList<Type> ExcludeCommonBDForDOs { get; set; }
        public static void AddComponentsTo(IWindsorContainer container, string[] domainModelAssemblies=null)
        {
            AddGenericDaosTo(container);
            //AddCustomDaosTo(container);
            AddQueryObjectsTo(container);
            AddDomainEvent(container);
            AddCommand(container);
            AddBusinessDelegateTo(container, domainModelAssemblies);
            AddControllerHelper(container);
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
        private static void AddBusinessDelegateTo(IWindsorContainer container,string[] domainModelAssemblies= null)
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
                  Component.For(typeof(IValidateController<,>))
                  .ImplementedBy(typeof(ValidateController<,>))
                  .LifestyleTransient());
            container.Register(
                Classes
                   .FromAssemblyInDirectory(allAssembly)
                   .BasedOn(typeof(IValidator<,>))
                   .WithService.AllInterfaces());

            if (IncludeCommonBDForDOs == null && ExcludeCommonBDForDOs==null)
            {
                container.Register(
                     Classes.FromAssemblyInDirectory(allAssembly)
                     .BasedOn(typeof(IBusinessDelegate))
                     .WithService.DefaultInterfaces());
            }
            else
            {//限制使用CommonBD类,只为指定的DO注册CommonBD. 
                //container.Register(
                //     Classes.FromAssemblyInDirectory(allAssembly)
                //     .BasedOn(typeof(IBusinessDelegate))
                //     .If(type => !type.FullName.StartsWith("ProjectBase.BusinessDelegate.CommonBD"))
                //     .WithService.DefaultInterfaces());

                ////keep following as the last one BD registeration, so derived class can be used first

                //if (IncludeCommonBDForDOs != null)
                //{
                //    foreach (var doClass in IncludeCommonBDForDOs)
                //    {
                //        RegisterCommonBDForDO(container, doClass);
                //    }
                //}
                //else if (ExcludeCommonBDForDOs != null && domainModelAssemblies!=null)
                //{
                //    var doClasses = new List<Type>();
                //    Array.ForEach(domainModelAssemblies, i => doClasses.AddRange(
                //        Assembly.LoadFrom(i).GetTypes().Where(
                //                t =>t.GetInterfaces().SingleOrDefault(o => o.Name.StartsWith("IEntityWithTypedId"))!=null
                //        )
                //    ));
                //    foreach (var doClass in doClasses)
                //    {
                //        if (ExcludeCommonBDForDOs.Contains(doClass)) continue;
                //        RegisterCommonBDForDO(container,doClass);
                //    }
                //}
            }
            //container.Register(
            //    Component.For(typeof(ICommandProcessor))
            //        .ImplementedBy(typeof(CommandProcessor))
            //        .Named("commandProcessor"));

            //container.Register(
            //    AllTypes.FromAssemblyNamed(ProjectHierarchy.BusinessDelegateNS)
            //        .BasedOn(typeof(ICommandHandler<>))
            //        .WithService.FirstInterface());

        }
        private static void RegisterCommonBDForDO(IWindsorContainer container, Type doClass)
        {
            var idType = doClass.GetInterfaces().Single(o => o.Name.StartsWith("IEntityWithTypedId")).GenericTypeArguments[0];
            var bdInterfaceType = typeof(ICommonBD<,>).MakeGenericType(doClass, idType);
            var bdImplType = typeof(CommonBD<,>).MakeGenericType(doClass, idType);
            container.Register(
                Component.For(bdInterfaceType)
                    .ImplementedBy(bdImplType));
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

            //container.Register(
            //    Component.For(typeof(IGenericDao<>))
            //        .ImplementedBy(typeof(BaseNHibernateLinqDaoWithTypedId<,int>))
            //        .Forward(typeof(INHibernateRepository<>))
            //        .Forward(typeof(ILinqRepository<>))
            //        .Forward(typeof(IRepository<>))
            //        .Named("nhibernateRepositoryType1"));
            //container.Register(
            //    Component.For(typeof(IGenericDaoWithAssignedIntId<>))
            //        .ImplementedBy(typeof(BaseNHibernateLinqDaoWithTypedId<,>))
            //        .Forward(typeof(INHibernateRepository<>))
            //        .Forward(typeof(ILinqRepository<>))
            //        .Forward(typeof(IRepository<>))
            //        .Named("nhibernateRepositoryType2"));

            //container.Register(
            //    Component.For(typeof(IGenericDaoWithAssignedGuid<>))
            //        .ImplementedBy(typeof(BaseNHibernateLinqDaoWithTypedId<,>))
            //        .Forward(typeof(INHibernateRepository<>))
            //        .Forward(typeof(ILinqRepository<>))
            //        .Forward(typeof(IRepository<>))
            //        .Named("nhibernateRepositoryType3"));

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
        private static void AddControllerHelper(IWindsorContainer container)
        {
            AssemblyFilter allAssembly = new AssemblyFilter(GetAssemblyPath());
            container.Register(
                Classes.FromAssemblyInDirectory(allAssembly)
                      .BasedOn(typeof(BaseControllerHelper))
            );
                      
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
