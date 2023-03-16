using ProjectBase.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectBase.Dto
{
    public static class SelfMergerExtention
    {
        /// <summary>
        /// 内部计算，无souce对象
        /// </summary>
        /// <param name="destinationList"></param>
        /// <param name="context"></param>
        public static void Merge(this IEnumerable<ISelfMerger> destinationList, BaseSelfMergerContext context = null)
        {
            var cnt = destinationList.Count();
            if (context == null)
            {
                for (var i = 0; i < cnt; i++)
                {
                    destinationList.ElementAt(i).Merge();
                }
            }
            else
            {
                for (var i = 0; i < cnt; i++)
                {
                    destinationList.ElementAt(i).Merge(context);
                }
            }
        }
        /// <summary>
        /// 从声明泛型时指定的类型的Source对象合并到自身；按集合索引顺序;一对一合并
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="destinationList"></param>
        /// <param name="sourceList"></param>
        /// <param name="context"></param>
        public static void Merge<TSource>(this IEnumerable<ISelfMerger<TSource>> destinationList, IEnumerable<TSource> sourceList, BaseSelfMergerContext context = null)
        {
            var cnt = destinationList.Count();
            if (context == null)
            {
                for (var i = 0; i < cnt; i++)
                {
                    destinationList.ElementAt(i).Merge(sourceList.ElementAt(i));
                }
            }
            else
            {
                for (var i = 0; i < cnt; i++)
                {
                    destinationList.ElementAt(i).Merge(sourceList.ElementAt(i), context);
                }
            }
        }
        /// <summary>
        /// 从任意Source对象合并到自身；按集合索引顺序
        /// </summary>
        /// <typeparam name="TVariantSource"></typeparam>
        /// <param name="destinationList"></param>
        /// <param name="sourceList"></param>
        /// <param name="context"></param>
        public static void MergeVar<TVariantSource>(this IEnumerable<ISelfMerger> destinationList, IEnumerable<TVariantSource> sourceList, BaseSelfMergerContext context = null)
        {
            var cnt = destinationList.Count();
            if (context == null)
            {
                for (var i = 0; i < cnt; i++)
                {
                    destinationList.ElementAt(i).MergeVar(sourceList.ElementAt(i));
                }
            }
            else
            {
                for (var i = 0; i < cnt; i++)
                {
                    destinationList.ElementAt(i).MergeVar(sourceList.ElementAt(i), context);
                }
            }
        }
   
        /// <summary>
        /// 从Source对象合并到自身；按指定匹配逻辑，匹配结果是一个集合，但只合并第一个元素
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="destinationList"></param>
        /// <param name="sourceList"></param>
        /// <param name="matcher"></param>
        /// <param name="context"></param>
        public static void Merge<TSource, TDestnation>(this IEnumerable<TDestnation> destinationList, IEnumerable<TSource> sourceList, Func<TSource, TDestnation, bool> matcher, BaseSelfMergerContext context = null) where TDestnation : ISelfMerger<TSource>
        {
            if (context == null)
            {
                foreach (var destItem in destinationList)
                {
                    destItem.Merge(sourceList.Where(o => matcher(o, destItem)).FirstOrDefault());
                }
            }
            else
            {
                foreach (var destItem in destinationList)
                {
                    destItem.Merge(sourceList.Where(o => matcher(o, destItem)).FirstOrDefault(), context);
                }
            }
        }
        /// <summary>
        /// 从任意Source对象合并到自身；按指定匹配逻辑，匹配结果是一个集合，合并整个集合
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestnation"></typeparam>
        /// <param name="destinationList"></param>
        /// <param name="sourceList"></param>
        /// <param name="matcher"></param>
        /// <param name="context"></param>
        public static void MergeList<TSource, TDestnation>(this IEnumerable<TDestnation> destinationList, IEnumerable<TSource> sourceList, Func<TSource, TDestnation, bool> matcher, BaseSelfMergerContext context = null) where TDestnation:ISelfMerger<TSource>
        {
            if (context == null)
            {
                foreach (var destItem in destinationList)
                {
                    destItem.MergeList(sourceList.Where(o => matcher(o, destItem)));
                }
            }
            else
            {
                foreach (var destItem in destinationList)
                {
                    destItem.MergeList(sourceList.Where(o => matcher(o, destItem)), context);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TVariantSource"></typeparam>
        /// <typeparam name="TDestnation"></typeparam>
        /// <param name="destinationList"></param>
        /// <param name="sourceList"></param>
        /// <param name="matcher"></param>
        /// <param name="context"></param>
        public static void MergeVarList<TVariantSource, TDestnation>(this IEnumerable<TDestnation> destinationList, IEnumerable<TVariantSource> sourceList, Func<TVariantSource, TDestnation, bool> matcher, BaseSelfMergerContext context = null) where TDestnation : ISelfMerger
        {
            if (context == null)
            {
                foreach (var destItem in destinationList)
                {
                    destItem.MergeVar(sourceList.Where(o => matcher(o, destItem)));
                }
            }
            else
            {
                foreach (var destItem in destinationList)
                {
                    destItem.MergeVar(sourceList.Where(o => matcher(o, destItem)), context);
                }
            }
        }
    }
}