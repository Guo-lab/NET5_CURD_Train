using AutoMapper;
using NHibernate.Util;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZXing.Common;

namespace ProjectBase.AutoMapper
{

    public class AutoMapperProfile : Profile
    {
        public static string[] DomainModelMappingAssemblies { get; set; }
        public AutoMapperProfile()
        {
//TODO:decimal转字符串格式处理不知道怎么做ForSourceType<decimal>().AddFormatExpression(context => ((decimal)context.SourceValue).ToString("c"));
            CreateMapForDORef();
        }

        protected void CreateMapForDORef() {
            var assemblies = new List<Assembly>();
            Array.ForEach(DomainModelMappingAssemblies, i => assemblies.Add(Assembly.LoadFrom(i)));

            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes()
                                .Where(t => (t.IsSubclassOf(typeof(BaseDomainObject)) ||
                                   t.IsSubclassOf(typeof(DomainObjectWithAssignedId<>))) &&
                                   Attribute.GetCustomAttribute(t, typeof(MappingIgnoreAttribute), false) == null
                                   );
                foreach (var t in types) {
                    if (t.IsSubclassOf(typeof(DomainObjectWithAssignedId<Guid>))) {
                        CreateMap(t, typeof(DORef<,>).MakeGenericType(new[] { t, typeof(Guid) }));
                    } else {
                        CreateMap(t, typeof(DORef<,>).MakeGenericType(new[] { t, typeof(int) }));
                    }
                }
            }

        }
    }

}
