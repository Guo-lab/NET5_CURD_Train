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
using SharpArch.Domain.DomainModel;
using ProjectBase.AutoMapper;

namespace ProjectBase.BusinessDelegate
{
    public class CommonBD<T, TId> : BaseBusinessDelegate, ICommonBD<T, TId> where T : BaseDomainObjectWithTypedId<TId>
                                                                          where TId : struct
    {
        private static char[] _caption = new char[]{
                                                           'A','B','C','D','E','F','G','H','I','J','K','L',
                                                           'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        public IGenericDaoWithTypedId<T, TId> Dao { get; set; }
        public IValidateController<T, TId> ValidateController { get; set; }
        protected virtual void Validate(T domainObject)
        {
            ValidateController.StartValidate(domainObject);
        }
        protected virtual void ValidateByAttribute(T domainObject, string attribute)
        {
            ValidateController.StartValidateByAttribute(domainObject, attribute);
        }

        public IUtil Util { get; set; }

        public CommonBD()
        {

        }

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

        public virtual void Save(T domainObject)
        {
            if (domainObject is IHasAssignedId)
            {
                if (domainObject.IsTransient())
                {
                    Dao.Save(domainObject);
                }
                else
                {
                    Dao.Update(domainObject);
                }
            }
            else
            {
                Dao.SaveOrUpdate(domainObject);
            }

            //         if (domainObject is DomainObjectWithAssignedIntId)
            //         {
            //             if (domainObject.IsTransient())
            //                 Dao.Save(domainObject);
            //             else
            //                 Dao.Update(domainObject);
            //         } else if (domainObject is DomainObjectWithUniqueId) {
            //             DomainObjectWithUniqueId t = domainObject as DomainObjectWithUniqueId;
            //             if (domainObject.IsTransient()) {
            //                 if (t.UniqueId == Guid.Empty) {
            //                     t.UniqueId =Guid.NewGuid();
            //                 } 
            //                 Dao.Save(domainObject);
            //             }
            //             else {
            //                 Dao.Update(domainObject);
            //             }
            //         }
            //         else if (domainObject is VersionedDomainObjectWithAssignedGuidIdId)
            //         {
            //             VersionedDomainObjectWithAssignedGuidIdId t = domainObject as VersionedDomainObjectWithAssignedGuidIdId;
            //             if (t.Id == Guid.Empty) t.SetAssignedIdTo(Guid.NewGuid());
            //             if (t.RowVersion == 0)
            //                 Dao.Save(domainObject);
            //             else
            //                 Dao.Update(domainObject);
            //         }else if (domainObject is DomainObjectWithAssignedGuidId)
            //{
            //             DomainObjectWithAssignedGuidId t = domainObject as DomainObjectWithAssignedGuidId;
            //             if (t.Id == Guid.Empty) t.SetAssignedIdTo(Guid.NewGuid());
            //             if (t.IsTransient())
            //                 Dao.Save(domainObject);
            //             else
            //                 Dao.Update(domainObject);
            //         }
            //         else
            //             Dao.SaveOrUpdate(domainObject);
        }

        public virtual void Delete(T domainObject)
        {
            Dao.Delete(domainObject);
        }
        public virtual void Delete(TId Id)
        {
            try
            {
                var domainObject = Dao.Load(Id);
                Dao.Delete(domainObject);
            }
            catch (ObjectNotFoundException)
            {

            }
        }
        public virtual int GetCountByQuery(Expression<Func<T, bool>> where)
        {
            return Dao.GetCountByQuery(where);
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
            return Dao.GetProjectionByQuery(o => o.Id.ToString() == Id.ToString(), null, o => new DORef<T, TId> { Id = o.Id, RefText = o.RefText }).FirstOrDefault();
        }
        public virtual IList<DORef<T, TId>> GetRefList(Expression<Func<T, bool>> where = null, string sort = null)
        {
            return Dao.GetProjectionByQuery(where, SortStruc<T>.CreateFrom(sort), o => new DORef<T, TId> { Id = o.Id, RefText = o.RefText });
        }
        public virtual IList<TDto> GetDtoList<TDto>(Pager pager, Expression<Func<T, bool>> where = null, string sort = null, Expression<Func<T, TDto>> selector = null)
        {
            if (selector == null)
            {
                Type dtoClass = typeof(TDto);
                IDictionary<string, Expression<Func<T, object>>> selectorMap = null;
                while (dtoClass != typeof(object))
                {
                    var staticField = dtoClass.GetField("SelectorMap", BindingFlags.Public | BindingFlags.Static);
                    if (staticField != null)
                    {
                        if (selectorMap == null) selectorMap = new Dictionary<string, Expression<Func<T, object>>>();
                        var map = (IDictionary<string, Expression<Func<T, object>>>)staticField.GetValue(null);
                        selectorMap.AddRange(map);
                    }
                    dtoClass = dtoClass.BaseType;
                }
                return GetDtoList2<TDto>(pager, where, sort, selectorMap);
            }
            if (pager == null)
            {
                return Dao.GetProjectionByQuery(where, SortStruc<T>.CreateFrom(sort), selector);
            }
            else
            {
                return Dao.GetProjectionByQuery(pager, where, SortStruc<T>.CreateFrom(sort), selector);
            }
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
                     IDictionary<string, Expression<Func<T, object>>> selectorMap = null)
        {
            return GetDtoListInternal<TDto>(pager, where, sort, selectorMap);
        }
        private IList<TDto> GetDtoListInternal<TDto>(Pager pager, Expression<Func<T, bool>> where, string sort,
           IDictionary<string, Expression<Func<T, object>>> selectorMap)
        {
            //following to make lamda like o=>new DTO{Id=o.Id,Other=o.Other}}
            var props = typeof(TDto).GetProperties().Where(o => o.CanWrite && !Attribute.IsDefined(o, typeof(SelectorIgnoreAttribute))).ToArray();
            MemberBinding[] propExprs = null;
            var pname = "o";
            if (where != null)
            {
                pname = where.Parameters[0].Name;
            }

            var parameterExpr = Expression.Parameter(typeof(T), pname);
            var newDtoExpr = Expression.New(typeof(TDto));//new TDto;

            Array.Resize(ref propExprs, props.Count());
            for (var i = 0; i < props.Count(); i++)//Id=o.Id;ReferenceId=o.Reference??null:o.Reference.Id
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
                }
                else
                {
                    entityPropExpr = ChainedPropExpr(parameterExpr, props[i].Name, typeof(T), props[i].PropertyType);
                }
                if (entityPropExpr == null) throw new Exception("can't figure out " + props[i].Name);
                MemberAssignment assign;
                try
                {
                    assign = Expression.Bind(props[i], entityPropExpr);
                }
                catch (ArgumentException e)
                {
                    throw new Exception("assignment error：" + props[i].Name + " " + e.Message);
                }
                propExprs[i] = assign;

            }
            var initExpr = Expression.MemberInit(newDtoExpr, propExprs);
            var selector = Expression.Lambda<Func<T, TDto>>(initExpr, new ParameterExpression[] { parameterExpr });
            if (pager == null)
            {
                return Dao.GetProjectionByQuery(where, SortStruc<T>.CreateFrom(sort), selector);
            }
            else
            {
                return Dao.GetProjectionByQuery(pager, where, SortStruc<T>.CreateFrom(sort), selector);
            }

        }
        private Expression ChainedPropExpr(Expression expr, string propName, Type containerType, Type dtoPropType)
        {
            if (propName == "") return null;
            var entityProp = containerType.GetProperty(propName);
            if (entityProp != null) return Expression.Property(expr, propName);
            var firstWordEnd = propName.IndexOfAny(_caption, 1);
            if (firstWordEnd <= 0) return null;
            var firstWord = propName.Substring(0, firstWordEnd);
            entityProp = containerType.GetProperty(firstWord);
            if (entityProp == null) return null;
            var firstPart = Expression.Property(expr, firstWord);
            var sencondPart = propName.Substring(firstWordEnd);
            if (dtoPropType.IsValueType && !Util.IsNullableType(dtoPropType))
                return ChainedPropExpr(firstPart, sencondPart, entityProp.PropertyType, dtoPropType);

            return Expression.Condition(Expression.Equal(firstPart, Expression.Constant(null)),
                 Expression.Constant(null, dtoPropType), Expression.Convert(ChainedPropExpr(firstPart, sencondPart, entityProp.PropertyType, dtoPropType), dtoPropType));
        }
        private Expression ChainedPropExprByMap(Expression expr, string propMappedName, Type containerType, Type dtoPropType)
        {
            if (propMappedName == "") return null;
            var entityProp = containerType.GetProperty(propMappedName);
            if (entityProp != null) return Expression.Property(expr, propMappedName);
            var firstWordEnd = propMappedName.IndexOf("|");
            if (firstWordEnd <= 0) return null;
            var firstWord = propMappedName.Substring(0, firstWordEnd);
            entityProp = containerType.GetProperty(firstWord);
            if (entityProp == null) return null;
            var firstPart = Expression.Property(expr, firstWord);
            var sencondPart = propMappedName.Substring(firstWordEnd + 1);
            if (dtoPropType.IsValueType && !Util.IsNullableType(dtoPropType))
                return ChainedPropExprByMap(firstPart, sencondPart, entityProp.PropertyType, dtoPropType);

            return Expression.Condition(Expression.Equal(firstPart, Expression.Constant(null)),
                Expression.Constant(null, dtoPropType), Expression.Convert(ChainedPropExprByMap(firstPart, sencondPart, entityProp.PropertyType, dtoPropType), dtoPropType));
        }

        public void ValidateAndSave(T domainObject, string validateAttribute = "")
        {
            if (string.IsNullOrEmpty(validateAttribute))
            {
                Validate(domainObject);
            }
            else
            {
                ValidateByAttribute(domainObject, validateAttribute);
            }
            Save(domainObject);
        }

 
    }
}
