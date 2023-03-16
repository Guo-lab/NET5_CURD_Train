using System;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace ProjectBase.Utils
{
    public class SortStruc<T>
    {
        public Expression<Func<T, Object>> OrderByExpression { get; set; }
        public OrderByDirectionEnum OrderByDirection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sort">should be a comma delimetered string</param>
        /// <returns></returns>
        public static SortStruc<T>[] CreateFrom(string sort)
        {
            if (sort == null) return null;
            if (string.IsNullOrEmpty(sort)) return new SortStruc<T>[0];

            var sortStrucList = sort.Split(',');
            var a = new SortStruc<T>[sortStrucList.Length];
            var i = 0;
            foreach (var sortStrucString in sortStrucList)
            {
                var sortStrucPair = sortStrucString.Split(' ');
                var direction = sortStrucPair.Length == 1 ? "asc" : sortStrucPair[1].ToLower();
                a[i++] = new SortStruc<T>
                                    {
                                        OrderByExpression =
                                            DynamicExpressionParser.ParseLambda<T, Object>(ParsingConfig.Default,false, sortStrucPair[0]),
                                        OrderByDirection =
                                            direction == "asc"
                                                ? OrderByDirectionEnum.Asc
                                                : OrderByDirectionEnum.Desc
                                    };
            }
            return a;
        }
        public static SortStruc<T>[] NoSort()
        {
            return new SortStruc<T>[0];
        }
        public static string ToString(SortStruc<T>[] sort)
        {
            var s = "";
            foreach (var part in sort)
            {
                var expr = part.OrderByExpression.Body;
                if (expr.NodeType == ExpressionType.Convert)
                {
                    expr = ((UnaryExpression)expr).Operand;
                }
                s += ","+ expr.ToString().Replace(part.OrderByExpression.Parameters[0].Name+".", "")
                    + " "+part.OrderByDirection.ToString();
            }
            return s.Substring(1);
        }
    }
}
