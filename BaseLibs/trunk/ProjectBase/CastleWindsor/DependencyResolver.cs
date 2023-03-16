using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectBase.CastleWindsor
{
    /// <summary>
    /// IOC依赖解析器。也包含相应的IOC注册方法。<br></br>
    /// 注册名计算逻辑用于支持测试：一个实现类与其TestDouble使用相同的注册名。
    /// </summary>
    public class DependencyResolver : ISubDependencyResolver
    {
        private IKernel kernel;

        private Func<string, string, string> calcResourceName = (serviceTypeName, implName) => serviceTypeName + "_impl_" + implName;

        /// <summary>
        /// 指定按类名注册时去掉开头字符串
        /// </summary>
        public string? NameExcludePrefix { get; set; }
        /// <summary>
        /// 指定按类名注册时去掉结尾字符串
        /// </summary>
        public string? NameExcludeSuffix { get; set; }

        /// <summary>
        /// 指定按类名注册时，从类名计算注册名的逻辑。<br></br>
        /// 缺省逻辑是如果指定了NameExcludePrefix/NameExcludeSuffix,则去掉相应部分。<br></br>
        /// 最简省的方式是什么都不指定，就使用类名作为注册名。这种情况下需要TestDouble的类名与原类同名（但ns不同）。
        /// </summary>
        public Func<string,string> CalcRegistryName { get; set; }

        public DependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
            CalcRegistryName = (implementationName) =>
            {
                if (NameExcludePrefix != null && implementationName.StartsWith(NameExcludePrefix))
                {
                    implementationName = implementationName.Substring(NameExcludePrefix.Length - 1);
                }
                if (NameExcludeSuffix != null && implementationName.EndsWith(NameExcludeSuffix))
                {
                    implementationName = implementationName.Substring(0, implementationName.IndexOf(NameExcludeSuffix) + 1);
                }
                return implementationName;
            };
        }

        /// <summary>
        /// 注册一个接口的所有实现类，以便可以通过[Resource]标记获得注入
        /// </summary>
        /// <param name="container"></param>
        /// <param name="allAssembly"></param>
        /// <param name="serviceType"></param>
        public void RegisterAllImplementations(IWindsorContainer container, AssemblyFilter allAssembly, Type serviceType, bool transient=false)
        {
            var reg = Classes.FromAssemblyInDirectory(allAssembly)
                    .BasedOn(serviceType)
                    .WithServiceSelf()
                    .Configure(o => o.Named(calcResourceName(serviceType.Name,CalcRegistryName(o.Implementation.Name))));
            if (transient)
            {
                reg = reg.LifestyleTransient();
            }
            container.Register(reg);
        }

        public object Resolve(string resourceName,Type serviceType)
        {
            return kernel.Resolve(calcResourceName(serviceType.Name, resourceName), serviceType);
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return GetResouceName(model, dependency) != null;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            var name = GetResouceName(model, dependency)!;

            return kernel.Resolve(calcResourceName(dependency.TargetType.Name,name), dependency.TargetType);
        }

        private string? GetResouceName(ComponentModel model, DependencyModel dependency)
        {
            var prop = model.Properties.Where(o => o.Dependency == dependency).Select(o => o.Property).SingleOrDefault();
            var injectAttr = prop?.GetCustomAttributes(false).OfType<ResourceAttribute>().SingleOrDefault();
            if (injectAttr != null) return injectAttr.Name ?? prop.Name;
            return null;
        }


    }
}
