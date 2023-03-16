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
    [MappingIgnore]
    public abstract class VersionedDomainObjectWithSeqId : BaseDomainObjectWithSeqId, IVersion
    {
        public virtual int RowVersion { get; set; }
    }

    [Serializable]
    public abstract class BaseDomainObject : BaseDomainObjectWithTypedId<int>
    {
    }

    /**
 * ͬ{@link BaseDomainObject}��ֻ��id�����㷨ΪӦ�ó���˳�����ɡ�
 * @author Rainy
 * @see --advanced
 */
    [Serializable]
    public abstract class BaseDomainObjectWithSeqId : BaseDomainObjectWithTypedId<int>
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

    //��ϵͳ���ݽ�����Domain����Id���Կ���Ϊ�������ͣ�Ϊ��ϵͳ���ܹ�����ʶ������һ��UniqueId����
    [Serializable]
    [MappingIgnore]
    public abstract class DomainObjectWithUniqueId : BaseDomainObject
    {
        public virtual Guid UniqueId { get; set; }
    }

}