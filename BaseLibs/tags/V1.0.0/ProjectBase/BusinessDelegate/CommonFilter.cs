using System;
using ProjectBase.Utils;
using ProjectBase.Domain;
using System.Linq.Expressions;

namespace ProjectBase.BusinessDelegate
{
    public class CommonFilter<T, TId>: ICommonFilter<T, TId> where T : BaseDomainObjectWithTypedId<TId>
    {
        public Expression<Func<T, bool>> Where { get; set; }

        
        public CommonFilter()
        {
            ResetFilter();
        }
        public void And(Expression<Func<T, bool>> expr)
        {
            this.Where = this.Where.And(expr);
        }

        public void ResetFilter()
        {
            this.Where = PredicateBuilder.True<T>();
        }
    }
}
