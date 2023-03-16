using NHibernate;
using NHibernate.Type;
using ProjectBase.Domain;
using SharpArch.Domain.DomainModel;
using System;

namespace ProjectBase.Data
{

    public class MyInterceptor : EmptyInterceptor
    {
        public override bool? IsTransient(object entity)
        {
            if (entity is IHasAssignedId)
            {
                return ((IHasAssignedId)entity).IsTransient();
            }
            else
            {
                return null;
            }
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity is IHasAssignedId)
            {
                ((IHasAssignedId)entity).OnSave();
            }
            if (entity is DomainObjectWithUniqueId)
            {
                DomainObjectWithUniqueId obj = (DomainObjectWithUniqueId)entity;
                if (obj.UniqueId == Guid.Empty)
                {
                    obj.UniqueId = Guid.NewGuid();
                }
            }
            return false;
        }
        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            return base.OnPrepareStatement(sql);

        }

    } 
}
