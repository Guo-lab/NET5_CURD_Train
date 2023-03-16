using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Transform;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain;
using SharpArch.NHibernate;

namespace ProjectBase.Utils
{
    public static class QueryExtension 
    {
        //no use for now
        public static IQueryable<TAny> Page<TAny>(this IQueryable<TAny> query,Pager pager)
        {
            Check.Require(pager != null, "pager may not be null!");
            //pager.ItemCount = query.Count();//need to set itemcount before call this method

            return pager.PageSize>0?query:query.Skip(pager.FromRowIndex).Take(pager.PageSize);

        }
        /// <summary>
        /// currently use linqtoobject to do paging,so all records will be loaded.
        /// this is only for embeded query to use,where nhibernate dosen't surport it.
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="list"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static IList<TAny> Page<TAny>(this IList<TAny> list, Pager pager)
        {
            Check.Require(pager != null, "pager may not be null!");
            pager.ItemCount = list.Count();

            return pager.PageSize>0?list: list.Skip(pager.FromRowIndex).Take(pager.PageSize).ToList();

        }
        /// <summary>
        /// currently use linqtoobject to do sorting,so all records will be loaded.
        /// this is only for use when subquery-ordering is involved,which nhibernate dosen't surport .
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortstring"></param>
        /// <returns></returns>
        public static IList<TAny> Sort<TAny>(this IList<TAny> list, string sortstring)
        {
            if (string.IsNullOrEmpty(sortstring)) return list;

            var sort = SortStruc<TAny>.CreateFrom(sortstring);
            
            if (sort.Length>0)
            {
                var orderedlist = sort[0].OrderByDirection == OrderByDirectionEnum.Asc
                        ? list.OrderBy(sort[0].OrderByExpression.Compile())
                        : list.OrderByDescending(sort[0].OrderByExpression.Compile());
                for(var i=1;i<sort.Length;i++)
                {
                    orderedlist = sort[i].OrderByDirection == OrderByDirectionEnum.Asc
                            ? orderedlist.ThenBy(sort[i].OrderByExpression.Compile())
                            : orderedlist.ThenByDescending(sort[i].OrderByExpression.Compile());
                }
                return orderedlist.ToList();
            }
            return list;
        }

        public static IQueryable<TAny> Sort<TAny>(this IQueryable<TAny> list, string sortstring)
        {
            if (string.IsNullOrEmpty(sortstring)) return list;

            var sort = SortStruc<TAny>.CreateFrom(sortstring);

            //see also BaseNHibernateLinqDao.Sort,here use ThenBy while there only use OrderBy,I think they are the same.
            if (sort.Length > 0)
            {
                var orderedlist = sort[0].OrderByDirection == OrderByDirectionEnum.Asc
                        ? list.OrderBy(sort[0].OrderByExpression)
                        : list.OrderByDescending(sort[0].OrderByExpression);
                for (var i = 1; i < sort.Length; i++)
                {
                    orderedlist = sort[i].OrderByDirection == OrderByDirectionEnum.Asc
                            ? orderedlist.ThenBy(sort[i].OrderByExpression)
                            : orderedlist.ThenByDescending(sort[i].OrderByExpression);
                }
                return orderedlist;
            }
            return list;
        }

        public static string ToSqlString(this string inputstring)
        {
            return inputstring == null ? null : inputstring.Replace("'", "''");
        }

        /// <summary>
        /// 查询条件中有一个大数组作为参数时，使用此方法。此方法切割数组后分次调用查询方法
        /// </summary>
        /// <typeparam name="TArrayEle"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="arrayParam">作为查询条件的参数的大数组</param>
        /// <param name="listGetFunc">查询数据的方法，接收切割后的数组作为查询条件</param>
        /// <param name="sliceSize">切割数组的大小</param>
        /// <returns></returns>
        public static IList<TDto> GetDtoListWithBigArrayParam<TArrayEle, TDto>(TArrayEle[] arrayParam,Func<IEnumerable<TArrayEle>, IList<TDto>> listGetFunc,int sliceSize=2000)
        {
            var targetList = new List<TDto>();
            int cnt = arrayParam.Length;
            int startIndex = 0;
            while (startIndex < cnt)
            {
                var partofArray = arrayParam.Skip(startIndex).Take(sliceSize);
                targetList.AddRange(listGetFunc(partofArray));
                startIndex += sliceSize;
            }
            return targetList;
        }
    }

}
