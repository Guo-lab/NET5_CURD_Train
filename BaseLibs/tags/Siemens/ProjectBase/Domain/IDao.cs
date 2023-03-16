using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Utils;
using SharpArch.NHibernate.Contracts.Repositories;
using System.Linq.Expressions;

namespace ProjectBase.Domain
{
    public interface IGenericDaoWithTypedId<T, TId> : INHibernateRepositoryWithTypedId<T, TId> where T: BaseDomainObjectWithTypedId<TId>
    {
        int GetCountByQuery(Expression<Func<T, bool>> where=null);
        [Obsolete("��ֹʹ��,�������������˼ӱ��")]
        IList<T> GetByQuery(Expression<Func<T, bool>> where, SortStruc<T>[] sort=null);
        [Obsolete("��ֹʹ��,�������������˼ӱ��")]
        IList<T> GetByQuery(Pager pager, Expression<Func<T, bool>> where=null, SortStruc<T>[] sort=null);
        T GetOneByQuery(Expression<Func<T, bool>> where, bool unique);
        T GetOneByQuery(Expression<Func<T, bool>> where, SortStruc<T>[] sort=null, bool unique=true);
        void Refresh(T entity);
        IQueryable<T> Query();
        IList<TResult> GetProjectionByQuery<TResult>(Func<IQueryable<TResult>, IQueryable<TResult>> queryBuilderInterceptor, Pager pager, Expression<Func<T, bool>> where, SortStruc<T>[] sort,
                                   Expression<Func<T, TResult>> selector);
        IList<TResult> GetProjectionByQuery<TResult>(Func<IQueryable<TResult>, IQueryable<TResult>> queryBuilderInterceptor, Expression<Func<T, bool>> where, SortStruc<T>[] sort,
                                           Expression<Func<T, TResult>> selector);
        IList<TResult> GetProjectionByQuery<TResult>(Pager pager, Expression<Func<T, bool>> where, SortStruc<T>[] sort,
                                   Expression<Func<T, TResult>> selector);
        IList<TResult> GetProjectionByQuery<TResult>(Expression<Func<T, bool>> where, SortStruc<T>[] sort,
                                           Expression<Func<T, TResult>> selector);
        IList<TResult> GetProjectionByQuery<TResult>(Pager pager, Expression<Func<T, bool>> where,
                           Expression<Func<T, TResult>> selector);
        IList<TResult> GetProjectionByQuery<TResult>(Expression<Func<T, bool>> where,
                                           Expression<Func<T, TResult>> selector);
        bool Exists(Expression<Func<T, bool>> where);
        //T GetWhole(TId id);//�ݲ�֧��
        //T GetWhole(TId id, String fetchProps);//�ݲ�֧��
        int Delete(Expression<Func<T, bool>> where);

        int Delete(TId entityId, Expression<Func<T, bool>> extraWhere);

        /// <summary>
        /// Finds an item by Id.
        /// </summary>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <param name="Id">The Id of the entity</param>
        /// <returns>The matching item</returns>
        //T FindOne(TId Id);

        /// <summary>
        /// Finds an item by a specification
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <returns>The the matching item</returns>
       // T FindOne(ILinqSpecification<T> specification);

        /// <summary>
        /// Finds all items within the repository.
        /// </summary>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <returns>All items in the repository</returns>
       // IQueryable<T> FindAll();

        /// <summary>
        /// Finds all items by a specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <returns>All matching items</returns>
       // IQueryable<T> FindAll(ILinqSpecification<T> specification);
    }

    [Obsolete("����ɾ����Ӧʹ��IGenericDaoWithTypedId<T, TId>")]
    public interface IGenericDao<T> : IGenericDaoWithTypedId<T, int> where T : BaseDomainObject 
    {
    }
    [Obsolete("����ɾ����Ӧʹ��IGenericDaoWithTypedId<T, TId>")]
    public interface IGenericDaoWithAssignedIntId<T>: IGenericDaoWithTypedId<T, int> where T : DomainObjectWithAssignedIntId
    {

    }
    [Obsolete("����ɾ����Ӧʹ��IGenericDaoWithTypedId<T, TId>")]
    public interface IGenericDaoWithAssignedGuid<T> : IGenericDaoWithTypedId<T, Guid> where T : VersionedDomainObjectWithAssignedGuidId
    {

    }

}
