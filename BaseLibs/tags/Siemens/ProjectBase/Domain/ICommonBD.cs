using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectBase.Domain
{
    public interface ICommonBD<T, TId> : ICommonReader<T, TId> where T : BaseDomainObjectWithTypedId<TId>
                                                           where TId :struct
    {
        void Save(T domainObject);
        void ValidateAndSave(T domainObject, string validateAttribute="");
        void Delete(T domainObject);
        void Delete(TId Id);
        int Delete(Expression<Func<T, bool>> where);

        int Delete(TId entityId, Expression<Func<T, bool>> extraWhere);
        void Refresh(T domainObject);

    }
}
    
