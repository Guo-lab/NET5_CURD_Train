using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
namespace ProjectBase.AutoMapper
{
    /// <summary>
    /// 此接口的实现类将由框架自动创建映射(CreateMap), 接口方法Map将在AutoMapper缺省映射赋值完成后被调用
    /// </summary>
    public interface ISelfMapper<TSource>
    {
        void Map(TSource src)
        {
            TypeExt.Map(src, this);
        }
    }

    public interface ISelfMapper<TSource, ContextT> : ISelfMapper<TSource> where ContextT : BaseSelfMapperContext
    {
        void Map(TSource src, ContextT context)
        {
            TypeExt.Map(src, this, context);
        }
    }
    /// <summary>
    /// 支持ISelfMapper嵌套映射
    /// </summary>
    public class TypeExt
    {
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static void Map<TSource>(TSource src, ISelfMapper<TSource> dest, BaseSelfMapperContext context = null)
        {
            var sourceProperties = typeof(TSource).GetProperties().Where(x => IsAssignableToGenericType(x.PropertyType, typeof(IList<>)));
            foreach (PropertyInfo pi in sourceProperties)
            {
                PropertyInfo destProperty = dest.GetType().GetProperty(pi.Name, BindingFlags.Instance | BindingFlags.Public);
                if (destProperty == null || IsAssignableToGenericType(destProperty.PropertyType, typeof(IList<>)) == false) { continue; }
                Type destUnderlyType = destProperty.PropertyType.GetGenericArguments()[0];

                Type sourceUnderlyType = pi.PropertyType.GetGenericArguments()[0];
                Type t1 = typeof(ISelfMapper<>).MakeGenericType(sourceUnderlyType);
                bool oneParameter = t1.IsAssignableFrom(destUnderlyType);
                Type t2 = typeof(ISelfMapper<,>).MakeGenericType(sourceUnderlyType, typeof(ISelfMapper<,>).GetGenericArguments()[1]);
                bool twoParameter = IsAssignableToGenericType(sourceUnderlyType, t2);
                if (oneParameter || twoParameter)
                {
                    var sourceCollection = (System.Collections.IList)pi.GetValue(src);
                    if (sourceCollection.Count == 0) { continue; }
                    var destCollection = (System.Collections.IList)destProperty.GetValue(dest);
                    MethodInfo methodInfo;
                    if (oneParameter)
                    {
                        methodInfo = (from method in destUnderlyType.GetMethods()
                                      where method.Name == "Map"
                                      let parameters = method.GetParameters()
                                      where parameters.Length == 1 && (
                                        parameters[0].ParameterType == sourceUnderlyType ||
                                        parameters[0].ParameterType.IsAssignableFrom(sourceUnderlyType)
                                      )
                                      select method).FirstOrDefault();
                        //如果类没有具体实现接口中的Map方法，尝试从接口中取得
                        if (methodInfo == null)
                        {
                            Type selfMapperInterface = destUnderlyType.GetInterfaces().First(x =>
                                x.IsGenericType &&
                                x.GetGenericTypeDefinition() == typeof(ISelfMapper<>) && 
                                (
                                    x.GetGenericArguments()[0]==sourceUnderlyType ||
                                    x.GetGenericArguments()[0].IsAssignableFrom(sourceUnderlyType)
                                )
                            );
                            methodInfo = (from method in selfMapperInterface.GetMethods()
                                          where method.Name == "Map"
                                          let parameters = method.GetParameters()
                                          where parameters.Length == 1 && (
                                            parameters[0].ParameterType == sourceUnderlyType ||
                                            parameters[0].ParameterType.IsAssignableFrom(sourceUnderlyType)
                                          )
                                          select method).FirstOrDefault();
                        }
                    }
                    else
                    {
                        methodInfo = (from method in destUnderlyType.GetMethods()
                                      where method.Name == "Map"
                                      let parameters = method.GetParameters()
                                      where parameters.Length == 2 && (
                                           parameters[0].ParameterType == sourceUnderlyType ||
                                           parameters[0].ParameterType.IsAssignableFrom(sourceUnderlyType)
                                      ) && typeof(BaseSelfMapperContext).IsAssignableFrom(parameters[1].ParameterType)
                                      select method).FirstOrDefault();
                        if (methodInfo == null)
                        {
                            Type selfMapperInterface = destUnderlyType.GetInterfaces().First(x =>
                                x.IsGenericType &&
                                x.GetGenericTypeDefinition() == typeof(ISelfMapper<,>) &&
                                (
                                   x.GetGenericArguments()[0] == sourceUnderlyType  ||
                                   x.GetGenericArguments()[0].IsAssignableFrom(sourceUnderlyType)
                                ) && typeof(BaseSelfMapperContext).IsAssignableFrom(x.GetGenericArguments()[1])
                            );
                            methodInfo = (from method in selfMapperInterface.GetMethods()
                                          where method.Name == "Map"
                                          let parameters = method.GetParameters()
                                          where parameters.Length == 2 && (
                                              parameters[0].ParameterType == sourceUnderlyType ||
                                              parameters[0].ParameterType.IsAssignableFrom(sourceUnderlyType)
                                          ) && typeof(BaseSelfMapperContext).IsAssignableFrom(parameters[1].ParameterType)
                                          select method).FirstOrDefault();
                        }
                    }
                    for (int i = 0; i < sourceCollection.Count; i++)
                    {
                        var source = sourceCollection[i];
                        var destination = destCollection[i];

                        if (oneParameter)
                        {
                            methodInfo.Invoke(destination, new object[] { source });
                        }
                        else if (context != null)
                        {
                            methodInfo.Invoke(destination, new object[] { source, context });
                        }
                        else
                        {
                            throw new Exception("MapperContext is required for " + sourceUnderlyType.Name + ".Map");
                        }
                    }
                }

            }
        }
    }

}
