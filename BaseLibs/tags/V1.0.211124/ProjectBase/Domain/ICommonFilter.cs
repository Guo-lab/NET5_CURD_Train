using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Utils;
using System.Linq.Expressions;

namespace ProjectBase.Domain
{
    public static class FilterExtension
    {
        public static Expression<Func<T, bool>> Reset<T>(this Expression<Func<T, bool>> where)
        {
            return PredicateBuilder.True<T>();
        }
    }
    public interface ICommonFilter<T,TId> where T : BaseDomainObjectWithTypedId<TId>
    {
        Expression<Func<T, bool>> Where { get; set; }

        void And(Expression<Func<T, bool>> expr);
        void Or(Expression<Func<T, bool>> expr);

        //void ResetFilter();
    }
}
