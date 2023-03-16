using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectBase.Domain
{

    public interface ICommonReader<T, TId> : IBusinessDelegate where T : BaseDomainObjectWithTypedId<TId>
                                                           where TId : struct
    {
        IGenericDaoWithTypedId<T, TId> Dao { set; }

        [Obsolete("禁止使用,暂留仅为兼容小房子")]
        IList<T> GetAll();

        /// <summary>
        /// 获取指定Id的记录，如果没有则抛出异常。如果id为null，返回null
        /// </summary>
        T Get(TId? Id);
        /// <summary>
        /// 获取指定Id的记录，如果没有返回null
        /// </summary>
        T TryGet(TId? Id);
        //T GetWhole(TId id);//暂不支持
        //T GetWhole(TId id, String fetchProps);//暂不支持
        int GetCountByQuery(Expression<Func<T, bool>> where=null);

        IList<T> GetByQuery(Expression<Func<T, bool>> where, string sort = null);

        IList<T> GetByQuery(Pager pager, Expression<Func<T, bool>> where = null, string sort = null);
        T GetOneByQuery(Expression<Func<T, bool>> where, bool unique);
        T GetOneByQuery(Expression<Func<T, bool>> where, string sort = null, bool unique = true);
        DORef<T, TId> GetRef(TId Id);
        IList<DORef<T, TId>> GetRefList(Expression<Func<T, bool>> where = null, string sort = null);
        IList<TDto> GetDtoList<TDto>(Pager pager, Expression<Func<T, bool>> where = null, string sort = null,
                             Expression<Func<T, TDto>> selector = null);
        IList<TDto> GetDtoList<TDto>(Expression<Func<T, bool>> where, string sort = null,
                     Expression<Func<T, TDto>> selector = null);
        IList<TDto> GetDtoList<TDto>(Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor, Pager pager, Expression<Func<T, bool>> where = null, string sort = null,
                     Expression<Func<T, TDto>> selector = null);
        IList<TDto> GetDtoList<TDto>(Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor, Expression<Func<T, bool>> where, string sort = null,
             Expression<Func<T, TDto>> selector = null);
        IList<TDto> GetDtoList2<TDto>(Pager pager, Expression<Func<T, bool>> where = null, string sort = null,
                            IDictionary<string, Expression<Func<T, object>>> selectorMap = null);
        IList<TDto> GetDtoList2<TDto>(Expression<Func<T, bool>> where, string sort = null,
                    IDictionary<string, Expression<Func<T, object>>> selectorMap = null);
        IQueryable<T> Query();

        IList<TDto> GetDtoList<TDto>(Expression<Func<T, TDto>> selector, Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor=null);

        bool Exists(Expression<Func<T, bool>> where);
        TDto GetDto<TDto>(TId Id);
        TDto GetDto<TDto>(Expression<Func<T, bool>> where = null, Expression<Func<T, TDto>> selector = null);
        TDto GetDto2<TDto>(Expression<Func<T, bool>> where = null, IDictionary<string, Expression<Func<T, object>>> selectorMap = null);
        TResult GetScalar<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> where);
    }
    [Obsolete("即将删除，应使用ICommonReader<T, TId>")]
    public interface ICommonReader<T, TId, FilterT> where T : BaseDomainObjectWithTypedId<TId>
                                                           where TId : struct
                                                           where FilterT : ICommonFilter<T, TId>
    {
        FilterT Filter { get; set; }
        //IList<T> GetAll();
        T Get(TId? Id);
        T TryGet(TId? Id);

        IList<T> List(string sort = null);
        IList<T> List(Pager pager, string sort = null);
        T GetOneRecord(bool unique = true);
        T GetOneRecord(string sort = null, bool unique = true);

        DORef<T, TId> GetRef(TId Id);
        IList<DORef<T, TId>> GetRefList(Expression<Func<T, bool>> where = null, string sort = null);
        IList<TDto> GetDtoList<TDto>(Pager pager, string sort = null, Expression<Func<T, TDto>> selector = null);
        IList<TDto> GetDtoList<TDto>(string sort, Expression<Func<T, TDto>> selector = null);

        IList<TDto> GetDtoList<TDto>(Expression<Func<T, TDto>> selector = null);
        IList<TDto> GetDtoList2<TDto>(Pager pager, string sort = null, IDictionary<string, Expression<Func<T, object>>> selectorMap = null);
        IList<TDto> GetDtoList2<TDto>(string sort, IDictionary<string, Expression<Func<T, object>>> selectorMap = null);
        IQueryable<T> Query();
        int Count();

        TDto GetDto<TDto>(TId Id);
        TDto GetDto<TDto>(Expression<Func<T, TDto>> selector = null);
        TDto GetDto2<TDto>(IDictionary<string, Expression<Func<T, object>>> selectorMap = null);
        TResult GetScalar<TResult>(Expression<Func<T, TResult>> selector);
    }
}

