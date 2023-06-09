﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Transform;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain;
using SharpArch.NHibernate;

namespace ProjectBase.Utils
{
    public static class QueryExtention 
    {
        //no use for now
        public static IQueryable<TAny> Page<TAny>(this IQueryable<TAny> query,Pager pager)
        {
            Check.Require(pager != null, "pager may not be null!");
            //pager.ItemCount = query.Count();//need to set itemcount before call this method

            return pager.PageSize==0?query:query.Skip(pager.FromRowIndex).Take(pager.PageSize);

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

            return pager.PageSize==0?list: list.Skip(pager.FromRowIndex).Take(pager.PageSize).ToList();

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
    }

}
