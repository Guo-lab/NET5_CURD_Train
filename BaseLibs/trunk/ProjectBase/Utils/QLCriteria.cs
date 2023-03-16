using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ProjectBase.Utils
{
    public class QLCriteria<T>
    {
        public Expression<Func<T, bool>> Expr { get; set; }
        public bool IsEmpty()
        {
            return Expr==null;
        }
        public static QLCriteria<T> Instance()
        {
            return new QLCriteria<T>();
        }
        public static QLCriteria<T> InitWhere(Expression<Func<T, bool>> initExpr)
        {
            var builder= Instance();
            builder.Expr = initExpr;
            return builder;
        }
        public QLCriteria<T> Or(Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, Expr.Parameters);
            Expr = Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(Expr.Body, invokedExpr), Expr.Parameters);
            return this;
        }
        public QLCriteria<T> And(Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, Expr.Parameters.Cast<Expression>());
            Expr = Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(Expr.Body, invokedExpr), Expr.Parameters);
            return this;
        }
        public QLCriteria<T> Not(Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, Expr.Parameters.Cast<Expression>());
            Expr = Expression.Lambda<Func<T, bool>>
                  (Expression.Not(Expr.Body), Expr.Parameters);
            return this;
        }
    }
}
