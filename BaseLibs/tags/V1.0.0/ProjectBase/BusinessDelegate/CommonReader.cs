using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate;
using ProjectBase.Domain;
using ProjectBase.Utils;
using System.Linq.Expressions;
using ProjectBase.Web.Mvc;
using System.Text;

namespace ProjectBase.BusinessDelegate
{
    //CommonReader并不实现ICommonReader，也就是不希望在UI层直接使用ICommonReader
    public class CommonReader<T, TId,FilterT> where T : BaseDomainObjectWithTypedId<TId>
                                              where TId : struct
                                              where FilterT :ICommonFilter<T,TId>
    {     
        public ICommonBD<T,TId> CommonBD { get; set; }
        private static char[] _caption = new char[]{
                                                           'A','B','C','D','E','F','G','H','I','J','K','L',
                                                           'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        public FilterT Filter { get; set; }

        //防止UI层直接生成CommonReader
        protected CommonReader()
        {
        }
       
       

        public int Count()
        {
           int r= CommonBD.GetCountByQuery(Filter.Where);
            Filter.Where = Filter.Where.Reset();
            return r;            
        }

        public T Get(TId? Id)
        {
            return CommonBD.Get(Id);
        }
        public T TryGet(TId? Id)
        {
            return CommonBD.TryGet(Id);
        }

        //protected void And(Expression<Func<T, bool>> expr)
        //{
        //    _filter = _filter.And(expr);
        //}
        //public void ResetFilter()
        //{
        //    _filter = PredicateBuilder.True<T>();
        //}

        public virtual IList<T> List()
        {
            IList<T> result = CommonBD.GetByQuery(Filter.Where);
            Filter.Where = Filter.Where.Reset();
            return result;
        }
        public virtual IList<T> List(string sort)
        {
            IList<T> result= CommonBD.GetByQuery(Filter.Where, sort);
            Filter.Where = Filter.Where.Reset();
            return result;
        }
        public virtual IList<T> List(Pager pager,string sort)
        {
            IList<T> result= CommonBD.GetByQuery(pager, Filter.Where, sort);
            Filter.Where = Filter.Where.Reset();
            return result;
        }
        public T GetOneRecord(bool unique = true)
        {
            T r= CommonBD.GetOneByQuery(Filter.Where, unique);
            Filter.Where = Filter.Where.Reset();
            return r;
        }
        public T GetOneRecord(string sort = null, bool unique = true)
        {
            T r = CommonBD.GetOneByQuery(Filter.Where, sort, unique);
            Filter.Where = Filter.Where.Reset();
            return r;
        }

        #region "Reserved"
        public virtual DORef<T, TId> GetRef(TId Id)
        {
            return CommonBD.GetRef(Id);
        }
        public virtual IList<DORef<T, TId>> GetRefList(Expression<Func<T, bool>> where = null, string sort = null)
        {
            return CommonBD.GetRefList(where,sort);
        }
        public virtual IList<TDto> GetDtoList<TDto>(Pager pager, string sort = null, Expression<Func<T, TDto>> selector = null)
        {
            return CommonBD.GetDtoList<TDto>(pager, Filter.Where, sort,selector);
        }
        public IQueryable<T> Query()
        {
            return CommonBD.Query();
        }

        /// <summary>
        /// automatically transform T to TDto,according to property-flatten pattern like automapper.(Use Caption to identiyfy words)
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="pager"></param>
        /// <param name="where"></param>
        /// <param name="sort"></param>
        /// <param name="propNameMap">use this to manually set mapping propname of DTO to prop of DO.words are delimetered by '|'
        /// </param>
        /// <returns></returns>
        public virtual IList<TDto> GetDtoList2<TDto>(Pager pager, string sort = null, IDictionary<string, Expression<Func<T, object>>> selectorMap = null)
        {
            return CommonBD.GetDtoList2<TDto>(pager, Filter.Where, sort, selectorMap);
        }
        #endregion

        }
}
