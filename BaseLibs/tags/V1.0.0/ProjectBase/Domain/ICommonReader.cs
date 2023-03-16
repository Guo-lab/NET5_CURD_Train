using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectBase.Utils;
using System.Linq.Expressions;
using ProjectBase.Web.Mvc;

namespace ProjectBase.Domain
{
    public interface ICommonReader<T, TId,FilterT>  where T : BaseDomainObjectWithTypedId<TId>
                                                           where TId :struct
                                                           where FilterT : ICommonFilter<T,TId>
    {
        FilterT Filter { get; set; }
        //IList<T> GetAll();
        T Get(TId? Id);
        T TryGet(TId? Id);

        //int GetCountByQuery(Expression<Func<T, bool>> where);
        //IList<T> GetByQuery(Expression<Func<T, bool>> where, string sort = null);
        //IList<T> GetByQuery(Pager pager, Expression<Func<T, bool>> where = null, string sort = null);
        //T GetOneByQuery(Expression<Func<T, bool>> where, bool unique);
        //T GetOneByQuery(Expression<Func<T, bool>> where, string sort = null, bool unique = true);

        IList<T> List(string sort = null);
        IList<T> List(Pager pager, string sort = null);
        T GetOneRecord(bool unique = true);
        T GetOneRecord(string sort = null, bool unique = true);

        DORef<T, TId> GetRef(TId Id);
        IList<DORef<T, TId>> GetRefList(Expression<Func<T, bool>> where = null, string sort = null);
        IList<TDto> GetDtoList<TDto>(Pager pager, string sort = null,
                             Expression<Func<T, TDto>> selector = null);
        IList<TDto> GetDtoList2<TDto>(Pager pager, string sort = null,
                            IDictionary<string, Expression<Func<T, object>>> selectorMap = null);
        IQueryable<T> Query();
        int Count();
    }
}
    
