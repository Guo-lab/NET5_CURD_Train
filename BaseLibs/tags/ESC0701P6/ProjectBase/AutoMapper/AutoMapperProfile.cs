using AutoMapper;
using NHibernate.Util;
using ProjectBase.Domain;
using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProjectBase.AutoMapper
{

    public class AutoMapperProfile : Profile
    {
        public static string[] DomainModelMappingAssemblies { get; set; }
        public static string[] ViewModelMappingAssemblies { get; set; }
        public AutoMapperProfile()
        {
            //TODO:decimal转字符串格式处理不知道怎么做ForSourceType<decimal>().AddFormatExpression(context => ((decimal)context.SourceValue).ToString("c"));
            CreateMapForDORef();
            CreateMapForSelfMapper();
        }

        protected void CreateMapForDORef()
        {
            if (DomainModelMappingAssemblies == null)
            {
                return;
            }
            var assemblies = new List<Assembly>();
            Array.ForEach(DomainModelMappingAssemblies, i => assemblies.Add(Assembly.LoadFrom(i)));

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                                .Where(t => 
                                    t.GetInterfaces().SingleOrDefault(o => o.Name.StartsWith("IEntityWithTypedId")) != null &&
                                    Attribute.GetCustomAttribute(t, typeof(MappingIgnoreAttribute), false) == null
                                );
                foreach (var t in types)
                {
                    if (t.IsSubclassOf(typeof(DomainObjectWithAssignedId<Guid>)))
                    {
                        CreateMap(t, typeof(DORef<,>).MakeGenericType(new[] { t, typeof(Guid) }));
                    }
                    else
                    {
                        CreateMap(t, typeof(DORef<,>).MakeGenericType(new[] { t, typeof(int) }));
                    }
                }
            }
        }
        protected void CreateMapForSelfMapper()
        {
            if (ViewModelMappingAssemblies == null)
            {
                return;
            }
            var interfaceName = "ProjectBase.AutoMapper.ISelfMapper";
            var assemblies = new List<Assembly>();
            Array.ForEach(ViewModelMappingAssemblies, i => assemblies.Add(Assembly.LoadFrom(i)));

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                                .Where(t => t.GetInterfaces().Any(f => f.FullName!=null && f.FullName.StartsWith(interfaceName)));
                foreach (var t in types)
                {
                    var srcTypeList = t.GetInterfaces().Where(f => f.FullName != null && f.FullName.StartsWith(interfaceName));
                    foreach (var srcType in srcTypeList)
                    {
                        CreateMap(srcType.GetGenericArguments()[0], t);
                    }
                }
            }
        }
    }

}
