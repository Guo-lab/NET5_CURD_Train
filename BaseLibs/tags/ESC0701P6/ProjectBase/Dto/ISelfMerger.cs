using System.Collections.Generic;

namespace ProjectBase.Dto
{
    public interface ISelfMerger<TSource>
    {
        void MergeList(IEnumerable<TSource> src);

        
        
    }

    //可以通过context参数传递与DTO无关的环境变量，例如当前登录账号
    public interface ISelfMerger<TSource, ContextT>:ISelfMerger<TSource> where ContextT : BaseSelfMergerContext
    {
        void MergeList(IEnumerable<TSource> src, ContextT context) 
        {
        }
    }

}