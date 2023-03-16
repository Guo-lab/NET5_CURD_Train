using AutoMapper;
using System;

namespace ProjectBase.AutoMapper
{
    /// <summary>
    /// 不要再使用，很快会被删除
    /// </summary>
    [Obsolete]
    public static class MapperExtension
    {
        ////同一个Domain映射成不同的DTO
        [Obsolete]
        public static void InheritMappingFromDestinationBaseType<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var sourceParentType = sourceType;// sourceType.BaseType;
            var destinationParentType = destinationType.BaseType;

            //mappingExpression
            //    .BeforeMap((x, y) => Mapper.Map(x, y, sourceParentType, destinationParentType))
            //    .ForAllMembers(x => x.Condition(r => NotAlreadyMapped(sourceParentType, destinationParentType, r)));
        }

        //Domain和DTO集成各自的父类的映射
        [Obsolete]
        public static void InheritMappingFromBaseType<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var sourceParentType = sourceType.BaseType;
            var destinationParentType = destinationType.BaseType;

            //mappingExpression
            //    .BeforeMap((x, y) => Mapper.Map(x, y, sourceParentType, destinationParentType))
            //    .ForAllMembers(x => x.Condition(r => NotAlreadyMapped(sourceParentType, destinationParentType, r)));
        }

        private static bool NotAlreadyMapped(Type sourceType, Type desitnationType, ResolutionContext r)
        {
            //return !r.IsSourceValueNull &&
            //       Mapper.FindTypeMapFor(sourceType, desitnationType).GetPropertyMaps().Where(
            //           m => m.DestinationProperty.Name.Equals(r.MemberName)).Select(y => !y.IsMapped()).All(b => b);
            return true;
        }
    }
}
