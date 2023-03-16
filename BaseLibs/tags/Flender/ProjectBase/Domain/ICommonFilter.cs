using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Utils;
using System.Linq.Expressions;

namespace ProjectBase.Domain
{
    [Obsolete("即将删除")]
    public static class FilterExtension
    {
        public static Expression<Func<T, bool>> Reset<T>(this Expression<Func<T, bool>> where)
        {
            return PredicateBuilder.True<T>();
        }
    }
    [Obsolete("即将删除。Filter类只有可能定制类，不再有CommonFilter")]
    public interface ICommonFilter<T,TId> where T : BaseDomainObjectWithTypedId<TId>
    {
        Expression<Func<T, bool>> Where { get; set; }

        void And(Expression<Func<T, bool>> expr);
        void Or(Expression<Func<T, bool>> expr);

        //void ResetFilter();
    }
}
