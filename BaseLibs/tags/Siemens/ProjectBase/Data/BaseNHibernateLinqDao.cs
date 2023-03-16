using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using ProjectBase.Utils;
using ProjectBase.Domain;
using SharpArch.Domain;
using SharpArch.NHibernate;
using System.Linq.Expressions;

namespace ProjectBase.Data
{
    public class BaseNHibernateLinqDaoWithTypedId<T, TId> : LinqRepositoryWithTypedId<T, TId>, IGenericDaoWithTypedId<T, TId> where T : BaseDomainObjectWithTypedId<TId>
    {

        private Type persitentType = typeof(T);

        public IQueryable<T> Query()
        {
            return Session.Query<T>();
        }
        
        public void Refresh(T entity)
        {
            try
            {
                Session.Refresh(entity, LockMode.Read);
            }
            catch (UnresolvableObjectException)
            {
                entity = GetById(entity.Id,false);
            }
        }
        /// <summary>
        /// Loads an instance of type T from the DB based on its ID.
        /// </summary>
        public T GetById(TId Id, bool shouldLock) {
            T entity;

            if (shouldLock) {
                entity = (T)this.Session.Load(persitentType, Id, LockMode.Upgrade);
            }
            else {
                entity = (T)this.Session.Load(persitentType, Id);
            }
            return entity;
        }

        /// <summary>
        /// Get item count by filter
        /// </summary>
        public int GetCountByQuery(Expression<Func<T, bool>> where)
        {
            if (where == null) return Session.Query<T>().Count();

            var c = Session.Query<T>().Count(where);
            return c;
        }

        /// <summary>
        /// Loads every instance of the requested type with no filtering,no paging
        /// </summary>
        public IList<T> GetByQuery(Expression<Func<T, bool>> where, SortStruc<T>[] sort)
        {
            return FilterSortQuery(where, sort).ToList();
        }

        public IList<T> GetByQuery(Pager pager, Expression<Func<T, bool>> where, SortStruc<T>[] sort)
        {
            return PagerFilterSortQuery(pager,where, sort).ToList();
        }
        /// <summary>
        /// Looks for a single instance using the query condition and check the total count.
        /// when being asked for an unique object but found more,throw an exception
        /// </summary>
        /// <param name="unique">if the result should be unique,default to true</param>
        /// <returns>the unique or the first object or defalt object</returns>
        /// <exception cref="NonUniqueResultException"></exception>
        public T GetOneByQuery(Expression<Func<T, bool>> where, SortStruc<T>[] sort=null, bool unique=true)
        {
            var q = Session.Query<T>();

            if (where != null) q = q.Where(where);
            if (unique) return q.SingleOrDefault();
            if (sort == null) sort = GetDefaultSort();
            if (sort != null)
            {
                foreach (var s in sort)
                {
                    q = s.OrderByDirection == OrderByDirectionEnum.Asc
                            ? q.OrderBy(s.OrderByExpression)
                            : q.OrderByDescending(s.OrderByExpression);
                }
            }
            return q.FirstOrDefault();
        }

        /// <summary>
        /// Looks for a single instance using the query condition and check the total count.
        /// </summary>
        /// <param name="unique">if the count must not be more than 1</param>
        /// <returns></returns>
        public T GetOneByQuery(Expression<Func<T, bool>> where, bool unique)
        {
            return GetOneByQuery(where, SortStruc<T>.NoSort(), unique);
        }
        public IList<TResult> GetProjectionByQuery<TResult>(Pager pager, Expression<Func<T, bool>> where, Expression<Func<T, TResult>> selector)
        {
            return GetProjectionByQuery(null, pager, where, SortStruc<T>.NoSort(), selector);
        }
        public IList<TResult> GetProjectionByQuery<TResult>(Expression<Func<T, bool>> where, Expression<Func<T, TResult>> selector)
        {
            return GetProjectionByQuery((Func<IQueryable<TResult>, IQueryable<TResult>>)null, where, SortStruc<T>.NoSort(), selector);
        }
        public IList<TResult> GetProjectionByQuery<TResult>(Pager pager, Expression<Func<T, bool>> where, SortStruc<T>[] sort, Expression<Func<T, TResult>> selector)
        {
            return GetProjectionByQuery(null,pager, where, sort, selector);
        }
        public IList<TResult> GetProjectionByQuery<TResult>(Expression<Func<T, bool>> where, SortStruc<T>[] sort, Expression<Func<T, TResult>> selector)
        {
            return GetProjectionByQuery((Func<IQueryable<TResult>, IQueryable<TResult>>)null, where, sort, selector);
        }
        public IList<TResult> GetProjectionByQuery<TResult>(Func<IQueryable<TResult>, IQueryable<TResult>> queryBuilderInterceptor, Expression<Func<T, bool>> where, SortStruc<T>[] sort, Expression<Func<T, TResult>> selector)
        {
            Check.Require(selector != null);
            var q = FilterSortQuery(where, sort).Select(selector);
            if (queryBuilderInterceptor != null)
            {
                q = queryBuilderInterceptor.Invoke(q);
            }
            return q.ToList();
        }
        public IList<TResult> GetProjectionByQuery<TResult>(Func<IQueryable<TResult>, IQueryable<TResult>> queryBuilderInterceptor, Pager pager, Expression<Func<T, bool>> where, SortStruc<T>[] sort, Expression<Func<T, TResult>> selector)
        {
            Check.Require(selector != null);
            Check.Require(pager != null, "pager may not be null!");

            pager.ItemCount = where == null ? Session.Query<T>().Count() : Session.Query<T>().Count(where);
            var q = FilterSortQuery(where, sort).Select(selector);
            if (queryBuilderInterceptor != null)
            {
                q = queryBuilderInterceptor.Invoke(q);
            }
            if (pager.PageSize > 0)
                q = q.Skip(pager.FromRowIndex).Take(pager.PageSize);
            return q.ToList();
        }
        private IQueryable<T> FilterSortQuery(Expression<Func<T, bool>> where, SortStruc<T>[] sort)
        {
            var q = Session.Query<T>();

            if (where != null) q = q.Where(where);
            if (sort == null) sort = GetDefaultSort();
            if (sort != null)
            {
                foreach (var s in sort)
                {
                    q = s.OrderByDirection == OrderByDirectionEnum.Asc
                            ? q.OrderBy(s.OrderByExpression)
                            : q.OrderByDescending(s.OrderByExpression);
                }
            }
            return q;
        }
        private IQueryable<T> PagerFilterSortQuery(Pager pager,Expression<Func<T, bool>> where, SortStruc<T>[] sort )
        {
            Check.Require(pager != null, "pager may not be null!");

            pager.ItemCount = where==null?Session.Query<T>().Count():Session.Query<T>().Count(where);
            var q = FilterSortQuery(where, sort);
            if (pager.PageSize > 0)
                return q.Skip(pager.FromRowIndex).Take(pager.PageSize);
            else
                return q;
        }

        private SortStruc<T>[] GetDefaultSort()
        {
            var field = typeof(T).GetField("DefaultSort");
            if(field==null) return null;
            try
            {
                return field.GetValue(null) as SortStruc<T>[];
            }
            catch(Exception)
            {
                throw new NetArchException("DO类中的DefaultSort定义错误");
            }
        }
 
        public  bool Exists(Expression<Func<T, bool>> where)
        {
            var one = Session.Query<T>().Where(where).Select(o=>o.Id).FirstOrDefault();
            return one!=null && !one.Equals(default(TId));
        }
        public int Delete(Expression<Func<T, bool>> where) 
        {
            Check.Require(where != null);
            return DeleteInternal(default(TId), where);
        }

        public int Delete(TId entityId, Expression<Func<T, bool>> extraWhere) 
        {
            Check.Require(entityId != null && extraWhere != null);
            return DeleteInternal(entityId, extraWhere);
        }

        private int DeleteInternal(TId entityId, Expression<Func<T, bool>> extraWhere)
        {
            Expression<Func<T, bool>> filter = extraWhere;
            if (entityId != null && !entityId.Equals(default(TId)))
            {
                var parameterExpr = Expression.Parameter(typeof(T), "o");
                var idFilter = Expression.Lambda<Func<T, bool>>(
                    Expression.Equal(Expression.Property(parameterExpr, "Id"), Expression.Constant(entityId)),
                    new ParameterExpression[] { parameterExpr });
                filter = idFilter.And(extraWhere);
            }
            return Session.Query<T>().Where(filter).Delete();
        }
    }
}
