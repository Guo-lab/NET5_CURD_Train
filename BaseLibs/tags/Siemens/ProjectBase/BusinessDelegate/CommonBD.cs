using NHibernate;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectBase.BusinessDelegate
{
    public class CommonBD<T, TId> : CommonReader<T, TId>, ICommonBD<T, TId> where T : BaseDomainObjectWithTypedId<TId>
                                                                          where TId : struct
    {
        private static char[] _caption = new char[]{
                                                           'A','B','C','D','E','F','G','H','I','J','K','L',
                                                           'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        public IValidateController<T, TId> ValidateController { get; set; }
        protected virtual void Validate(T domainObject)
        {
            ValidateController.StartValidate(domainObject);
        }
        protected virtual void ValidateByAttribute(T domainObject, string attribute)
        {
            ValidateController.StartValidateByAttribute(domainObject, attribute);
        }

        public CommonBD()
        {

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
        public virtual int Delete(Expression<Func<T, bool>> where)
        {
            return Dao.Delete(where);
        }

        public virtual int Delete(TId entityId, Expression<Func<T, bool>> extraWhere)
        {
            return Dao.Delete(entityId, extraWhere);
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

        public void Refresh(T domainObject)
        {
            Dao.Refresh(domainObject);
        }
    }
}
