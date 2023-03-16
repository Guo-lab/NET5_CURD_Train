using System.Collections.Generic;

namespace ProjectBase.Dto
{
    /// <summary>
    /// 此接口的实现类对自己的属性进行计算赋值，或者以外来参数对象为数据来源为自己的属性计算赋值
    /// </summary>
    public interface ISelfMerger
    {
        void MergeVar<TVariantSource>(TVariantSource src)
        {
        }
        void MergeVar<TVariantSource>(TVariantSource src, BaseSelfMergerContext context)
        {
        }
        void Merge()
        {
        }
        void Merge(BaseSelfMergerContext context)
        {
        }
    }
    /// <summary>
    /// ISelfMerger子类，扩展了两个方法使用固定的外来参数类型。
    /// </summary>
    public interface ISelfMerger<TSource> : ISelfMerger
    {
        void Merge(TSource src)
        {
        }
        void Merge(TSource src, BaseSelfMergerContext context)
        {
        }
        void MergeList(IEnumerable<TSource> src)
        {
        }
        void MergeList(IEnumerable<TSource> src,BaseSelfMergerContext context)
        {
        }
    }

}