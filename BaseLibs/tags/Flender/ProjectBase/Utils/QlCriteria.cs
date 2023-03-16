using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ProjectBase.Utils
{
    public class QlCriteria<T>
    {
        public Expression<Func<T, bool>> Expr { get; set; }
        public bool IsEmpty()
        {
            return Expr==null;
        }
        public static QlCriteria<T> Instance()
        {
            return new QlCriteria<T>();
        }
        public static QlCriteria<T> InitWhere(Expression<Func<T, bool>> initExpr)
        {
            var builder= Instance();
            builder.Expr = initExpr;
            return builder;
        }
        public QlCriteria<T> Or(Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, Expr.Parameters);
            Expr = Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(Expr.Body, invokedExpr), Expr.Parameters);
            return this;
        }
        public QlCriteria<T> And(Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, Expr.Parameters.Cast<Expression>());
            Expr = Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(Expr.Body, invokedExpr), Expr.Parameters);
            return this;
        }
        public QlCriteria<T> Not(Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, Expr.Parameters.Cast<Expression>());
            Expr = Expression.Lambda<Func<T, bool>>
                  (Expression.Not(Expr.Body), Expr.Parameters);
            return this;
        }
    }
}
