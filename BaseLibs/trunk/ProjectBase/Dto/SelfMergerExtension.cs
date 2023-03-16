using ProjectBase.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectBase.Dto
{
    public static class SelfMergerExtention
    {

        /// <summary>
        /// 从任意Source对象合并到自身；按指定匹配逻辑，匹配结果是一个集合，合并整个集合
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestnation"></typeparam>
        /// <param name="destinationList"></param>
        /// <param name="sourceList"></param>
        /// <param name="matcher"></param>
        public static void MergeList<TSource, TDestnation>(this IEnumerable<TDestnation> destinationList, IEnumerable<TSource> sourceList, Func<TSource, TDestnation, bool> matcher) where TDestnation : ISelfMerger<TSource>
        {

            foreach (var destItem in destinationList)
            {
                destItem.MergeList(sourceList.Where(o => matcher(o, destItem)));
            }

        }

        public static void MergeList<TSource, TDestnation, TContext>(this IEnumerable<TDestnation> destinationList,
                                                                                                 IEnumerable<TSource> sourceList,
                                                                                                 Func<TSource, TDestnation, bool> matcher,
                                                                                                 TContext context) where TDestnation : ISelfMerger<TSource,TContext>
                                                                                                                            where TContext : BaseSelfMergerContext
        {
            foreach (var destItem in destinationList)
            {
                destItem.MergeList(sourceList.Where(o => matcher(o, destItem)), context);
            }
        }


    }
}