using ProjectBase.Domain.NhibernateMapByCode;
using System;
using SharpArch.Domain.DomainModel;

namespace ProjectBase.Domain
{
    [Serializable]
    [MappingIgnore]
    public abstract class VersionedDomainObject : BaseDomainObject,IVersion
    {
        public virtual int RowVersion { get; set; }
    }

    [Serializable]
    public abstract class BaseDomainObject : BaseDomainObjectWithTypedId<int>
    {
    }
      
    [Serializable]
    public abstract class BaseDomainObjectWithTypedId<TId> : EntityWithTypedId<TId>
    {
        public virtual string RefText { get; protected set; }

        public override string ToString()
        {
            string s = "";
            var signatures = GetTypeSpecificSignatureProperties();
            foreach (var prop in signatures)
            {
                s += prop.GetValue(this, null) + "-";
            }
            if (s=="") return base.ToString();
            return s.Remove(s.Length - 1);
        }

    }

    //跨系统数据交换的Domain对象，Id属性可能为任意类型，为了系统间能够互相识别，增加一个UniqueId属性
    [Serializable]
    [MappingIgnore]
    public abstract class DomainObjectWithUniqueId : BaseDomainObject
    {
        public virtual Guid UniqueId { get; set; }
    }

}