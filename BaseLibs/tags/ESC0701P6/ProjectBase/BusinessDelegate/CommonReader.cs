using ProjectBase.Domain;
using ProjectBase.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectBase.BusinessDelegate
{
    //CommonReader并不实现ICommonReader，也就是不希望在UI层直接使用ICommonReader
    [Obsolete("弃用。使用CommonReader<T, TId>替代")]
    public class CommonReader<T, TId,FilterT> where T : BaseDomainObjectWithTypedId<TId>
                                              where TId : struct
                                              where FilterT :ICommonFilter<T,TId>
    {     
        public ICommonBD<T,TId> CommonBD { get; set; }

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
            IList<DORef<T, TId>> r = CommonBD.GetRefList(where,sort);
            Filter.Where = Filter.Where.Reset();
            return r;
        }
        public virtual IList<TDto> GetDtoList<TDto>(Pager pager, string sort = null, Expression<Func<T, TDto>> selector = null)
        {
            IList<TDto> r= CommonBD.GetDtoList<TDto>(pager, Filter.Where, sort,selector);
            Filter.Where = Filter.Where.Reset();
            return r;
        }
        public virtual IList<TDto> GetDtoList<TDto>(string sort, Expression<Func<T, TDto>> selector = null)
        {
            return GetDtoList(null, sort, selector);
        }

        public virtual IList<TDto> GetDtoList<TDto>(Expression<Func<T, TDto>> selector = null)
        {
            return GetDtoList(null, null, selector);
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
            IList<TDto> r = CommonBD.GetDtoList2<TDto>(pager, Filter.Where, sort, selectorMap);
            Filter.Where = Filter.Where.Reset();
            return r;
        }
        public virtual IList<TDto> GetDtoList2<TDto>(string sort, IDictionary<string, Expression<Func<T, object>>> selectorMap = null)
        {
            IList<TDto> r = CommonBD.GetDtoList2<TDto>(Filter.Where, sort, selectorMap);
            Filter.Where = Filter.Where.Reset();
            return r;
        }

        public TDto GetDto<TDto>(TId Id) {
            return CommonBD.GetDto<TDto>(Id);
        }
        public TDto GetDto<TDto>(Expression<Func<T, TDto>> selector = null) {
            TDto dto = CommonBD.GetDto<TDto>(Filter.Where, selector);
            Filter.Where = Filter.Where.Reset();
            return dto;
        }
        public TDto GetDto2<TDto>(IDictionary<string, Expression<Func<T, object>>> selectorMap = null) {
            TDto dto = CommonBD.GetDto2<TDto>(Filter.Where, selectorMap);
            Filter.Where = Filter.Where.Reset();
            return dto;
        }
        public TResult GetScalar<TResult>(Expression<Func<T, TResult>> selector) {
            TResult result = CommonBD.GetScalar(selector, Filter.Where);
            Filter.Where = Filter.Where.Reset();
            return result;
        }

        #endregion

    }

    public class CommonReader<T, TId> : BaseBusinessDelegate, ICommonReader<T, TId> where T : BaseDomainObjectWithTypedId<TId>
                                              where TId : struct
    {
        private static char[] _caption = new char[]{
                                                           'A','B','C','D','E','F','G','H','I','J','K','L',
                                                           'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };
        private IGenericDaoWithTypedId<T, TId> dao;
        public IGenericDaoWithTypedId<T, TId> Dao { 
            protected get
            {
                return dao;
            }
            set
            {
                dao = value;
            }
        }
        public IUtil Util { get; set; }
 
        public virtual IList<T> GetAll()
        {
            return Dao.GetAll();
        }

        public virtual T Get(TId? Id)
        {
            if (!Id.HasValue) return null;
            return Dao.Load(Id.Value);
        }

        public T TryGet(TId? Id)
        {
            if (!Id.HasValue) return null;
            return Dao.Get(Id.Value);
        }
        public virtual int GetCountByQuery(Expression<Func<T, bool>> where=null)
        {
            return Dao.GetCountByQuery(where);
        }

        public bool Exists(Expression<Func<T, bool>> where)
        {
            return Dao.Exists(where);
        }
        //public T GetWhole(TId id)//暂不支持
        //{
        //    return Dao.GetWhole(id);
        //}
        //public T GetWhole(TId id, String fetchProps)//暂不支持
        //{
        //    return Dao.GetWhole(id, fetchProps);
        //}
        public TDto GetDto<TDto>(TId Id)
        {
            var parameterExpr = Expression.Parameter(typeof(T), "o");
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(Expression.Property(parameterExpr, "Id"), Expression.Constant(Id)),
                new ParameterExpression[] { parameterExpr });
            var rtn = GetDto<TDto>(where);
            if (rtn == null) throw new NetArchException("数据库中不存在指定Id的记录：Id=" + Id.ToString());
            return rtn;
        }
        public TDto GetDto<TDto>(Expression<Func<T, bool>> where = null, Expression<Func<T, TDto>> selector = null)
        {
            var list = GetDtoList(where, "", selector);
            if (list.Count > 1)
            {
                throw new Exception("查询到的记录不唯一");
            }
            return list.Count == 0 ? default(TDto) : list[0];
        }
        public TDto GetDto2<TDto>(Expression<Func<T, bool>> where = null, IDictionary<string, Expression<Func<T, object>>> selectorMap = null)
        {
            var list = GetDtoList2<TDto>(where, "", selectorMap);
            if (list.Count > 1)
            {
                throw new Exception("查询到的记录不唯一");
            }
            return list.Count == 0 ? default(TDto) : list[0];
        }
        public TResult GetScalar<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> where)
        {
            var list = Dao.GetProjectionByQuery(where, SortStruc<T>.CreateFrom(""), selector);
            if (list.Count > 1)
            {
                throw new Exception("查询到的记录不唯一");
            }
            return list.Count == 0 ? default(TResult) : list[0];
        }
        public virtual IList<T> GetByQuery(Expression<Func<T, bool>> where, string sort = null)
        {
            return Dao.GetByQuery(where, SortStruc<T>.CreateFrom(sort));
        }
        public virtual IList<T> GetByQuery(Pager pager, Expression<Func<T, bool>> where = null, string sort = null)
        {
            return Dao.GetByQuery(pager, where, SortStruc<T>.CreateFrom(sort));
        }
        public virtual T GetOneByQuery(Expression<Func<T, bool>> where = null, bool unique = true)
        {
            return Dao.GetOneByQuery(where, unique);
        }
        public virtual T GetOneByQuery(Expression<Func<T, bool>> where, string sort = null, bool unique = true)
        {
            return Dao.GetOneByQuery(where, SortStruc<T>.CreateFrom(sort), unique);
        }
        public virtual DORef<T, TId> GetRef(TId Id)
        {
            return Dao.GetProjectionByQuery(o => o.Id.ToString() == Id.ToString(), SortStruc<T>.CreateFrom(""), o => new DORef<T, TId> { Id = o.Id, RefText = o.RefText }).FirstOrDefault();
        }
        public virtual IList<DORef<T, TId>> GetRefList(Expression<Func<T, bool>> where = null, string sort = null)
        {
            return Dao.GetProjectionByQuery(where, SortStruc<T>.CreateFrom(sort), o => new DORef<T, TId> { Id = o.Id, RefText = o.RefText });
        }
        public virtual IList<TDto> GetDtoList<TDto>(Expression<Func<T, bool>> where, string sort = null, Expression<Func<T, TDto>> selector = null)
        {
            return GetDtoList(null, null, where, sort, selector);
        }
        public virtual IList<TDto> GetDtoList<TDto>(Pager pager, Expression<Func<T, bool>> where = null, string sort = null, Expression<Func<T, TDto>> selector = null)
        {
            return GetDtoList(null, pager, where, sort, selector);
        }
        public IList<TDto> GetDtoList<TDto>(Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor, Expression<Func<T, bool>> where, string sort = null,
            Expression<Func<T, TDto>> selector = null)
        {
            return GetDtoList(queryBuilderInterceptor, null, where, sort, selector);
        }
        public IList<TDto> GetDtoList<TDto>(Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor, Pager pager, Expression<Func<T, bool>> where = null, string sort = null,
             Expression<Func<T, TDto>> selector = null)
        {
            if (selector == null)
            {
                Type dtoClass = typeof(TDto);
                IDictionary<string, Expression<Func<T, object?>>> selectorMap = null;
                while (dtoClass != typeof(object))
                {
                    var staticField = dtoClass.GetField("SelectorMap", BindingFlags.Public | BindingFlags.Static);
                    if (staticField != null)
                    {
                        if (selectorMap == null) selectorMap = new Dictionary<string, Expression<Func<T, object?>>>();
                        var map = (IDictionary<string, Expression<Func<T, object?>>>)staticField.GetValue(null);
                        selectorMap.AddRange(map);
                    }
                    dtoClass = dtoClass.BaseType;
                }
                return GetDtoList2(queryBuilderInterceptor, pager, where, sort, selectorMap);
            }
            if (pager == null)
            {
                return Dao.GetProjectionByQuery(queryBuilderInterceptor, where, SortStruc<T>.CreateFrom(sort), selector);
            }
            else
            {
                return Dao.GetProjectionByQuery(queryBuilderInterceptor, pager, where, SortStruc<T>.CreateFrom(sort), selector);
            }
        }
        public IList<TDto> GetDtoList<TDto>(Expression<Func<T, TDto>> selector, Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor = null)
        {
            return GetDtoList(queryBuilderInterceptor, null, "", selector);
        }
        public IQueryable<T> Query()
        {
            return Dao.Query();
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
        public IList<TDto> GetDtoList2<TDto>(Pager pager, Expression<Func<T, bool>> where = null, string sort = null,
                    IDictionary<string, Expression<Func<T, object?>>> selectorMap = null)
        {
            return GetDtoListInternal<TDto>(pager, where, sort, selectorMap, null);
        }
        public IList<TDto> GetDtoList2<TDto>(Expression<Func<T, bool>> where, string sort = null,
            IDictionary<string, Expression<Func<T, object?>>> selectorMap = null)
        {
            return GetDtoListInternal<TDto>(null, where, sort, selectorMap, null);
        }
        public IList<TDto> GetDtoList2<TDto>(Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor, Expression<Func<T, bool>> where, string sort = null,
            IDictionary<string, Expression<Func<T, object?>>> selectorMap = null)
        {
            return GetDtoListInternal(null, where, sort, selectorMap, queryBuilderInterceptor);
        }
        public IList<TDto> GetDtoList2<TDto>(Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor, Pager pager, Expression<Func<T, bool>> where = null, string sort = null,
            IDictionary<string, Expression<Func<T, object?>>> selectorMap = null)
        {
            return GetDtoListInternal(pager, where, sort, selectorMap, queryBuilderInterceptor);
        }

        private IList<TDto> GetDtoListInternal<TDto>(Pager pager, Expression<Func<T, bool>> where, string sort,
           IDictionary<string, Expression<Func<T, object?>>> selectorMap, Func<IQueryable<TDto>, IQueryable<TDto>> queryBuilderInterceptor)
        {
            //following to make lamda like o=>new DTO{Id=o.Id,Other=o.Other}}
            var props = typeof(TDto).GetProperties().Where(o => o.CanWrite
                && !o.PropertyType.IsArray
                && (!o.PropertyType.IsAssignableTo(typeof(IEnumerable))|| o.PropertyType==typeof(string))
                && !Attribute.IsDefined(o, typeof(SelectorIgnoreAttribute))).ToArray();
            MemberBinding[] propExprs = null;
            var pname = "o";
            if (where != null)
            {
                pname = where.Parameters[0].Name;
            }

            var parameterExpr = Expression.Parameter(typeof(T), pname);
            var newDtoExpr = Expression.New(typeof(TDto));//new TDto;

            Array.Resize(ref propExprs, props.Count());
            for (var i = 0; i < props.Count(); i++)
            {
                Expression entityPropExpr;
                if (selectorMap != null && selectorMap.ContainsKey(props[i].Name))
                {
                    var expr = selectorMap[props[i].Name];
                    entityPropExpr = expr.Body.Replace(expr.Parameters[0], parameterExpr);
                    if (entityPropExpr.NodeType == ExpressionType.Convert)
                    {
                        entityPropExpr = ((UnaryExpression)entityPropExpr).Operand;
                    }
                    entityPropExpr = Convert(entityPropExpr, props[i].PropertyType, props[i].Name);
                }
                else
                {
                    entityPropExpr = ChainedPropExpr(parameterExpr, props[i].Name, typeof(T), props[i].PropertyType);
                }
                if (entityPropExpr == null) throw new NetArchException("无法从DTO对象的属性名推断出对应的Select表达式：" + props[i].Name);

                MemberAssignment assign;
                try
                {
                    assign = Expression.Bind(props[i], entityPropExpr);
                }
                catch (ArgumentException e)
                {
                    throw new Exception("框架内部代码异常：" + props[i].Name + " " + e.Message);
                }
                propExprs[i] = assign;

            }
            var initExpr = Expression.MemberInit(newDtoExpr, propExprs);
            var selector = Expression.Lambda<Func<T, TDto>>(initExpr, new ParameterExpression[] { parameterExpr });
            if (pager == null)
            {
                return Dao.GetProjectionByQuery(queryBuilderInterceptor, where, SortStruc<T>.CreateFrom(sort), selector);
            }
            else
            {
                return Dao.GetProjectionByQuery(queryBuilderInterceptor, pager, where, SortStruc<T>.CreateFrom(sort), selector);
            }

        }
        private Expression ChainedPropExpr(Expression expr, string propName, Type containerType, Type dtoPropType)
        {
            if (propName == "") return null;
            Expression rtn = null;
            var entityProp = containerType.GetProperty(propName);
            if (entityProp != null)
            {
                rtn = Expression.Property(expr, propName);
                rtn = EntityPropExpr4DORef(rtn, dtoPropType);
                return Convert(rtn, dtoPropType, propName);
            }
            MemberExpression firstPart;
            var startIndex = 0;
            while (true)
            {
                var firstWordEnd = propName.IndexOfAny(_caption, startIndex + 1);
                if (firstWordEnd <= 0) return null;
                var firstWord = propName.Substring(0, firstWordEnd);
                entityProp = containerType.GetProperty(firstWord);
                if (entityProp == null)//前一个单词识别不成，就识别前两个单词
                {
                    startIndex = firstWordEnd;
                    continue;
                }
                firstPart = Expression.Property(expr, firstWord);
                var sencondPart = propName.Substring(firstWordEnd);
                rtn = ChainedPropExpr(firstPart, sencondPart, entityProp.PropertyType, dtoPropType);
                return rtn;
            }
        }
        private Expression Convert(Expression rtn,Type dtoPropType,string propName)
        {
            if (Util.IsNullableType(rtn.Type) && !Util.IsNullableType(dtoPropType))
            {
                throw new NetArchException("从DomainObject到DTO的赋值类型错误，不能从可空类型赋值到不可空类型：" + propName);
            }
            return Expression.Convert(rtn, dtoPropType);
        }
        private bool IsEntityPropTypeEnum(PropertyInfo entityProp)
        {
            return entityProp.PropertyType.IsEnum || Util.IsNullableType(entityProp.PropertyType) && Nullable.GetUnderlyingType(entityProp.PropertyType).IsEnum;
        }
        private Expression EntityPropExpr4DORef(Expression entityPropExpr, Type dtoPropType)
        { //Id=o.Id;ReferenceId=o.Reference??null:o.Reference.Id
            if (!dtoPropType.Name.StartsWith("DORef")) return entityPropExpr;

            var newDORefExpr = Expression.New(dtoPropType);//new DORef;
            MemberAssignment[] propAssign = new MemberAssignment[2];
            try
            {
                //此处注意要对Id的类型进行转换
                var idProp = dtoPropType.GetProperty("Id");
                propAssign[0] = Expression.Bind(idProp, Expression.Convert(Expression.Property(entityPropExpr, "Id"), idProp.PropertyType));
                propAssign[1] = Expression.Bind(dtoPropType.GetProperty("RefText"), Expression.Property(entityPropExpr, "RefText"));
            }
            catch (Exception)
            {
                throw new Exception("DORef类型转换错");
            }
            return Expression.MemberInit(newDORefExpr, propAssign);
        }
    }
}
