using ProjectBase.Domain.NhibernateMapByCode;
using SharpArch.Domain.DomainModel;
using System;

namespace ProjectBase.Domain
{
    [Serializable]
    [MappingIgnore]
    public abstract class DomainObjectWithAssignedId<IdT> : BaseDomainObjectWithTypedId<IdT>,IHasAssignedId<IdT>,IHasAssignedId
    {
        private bool _isTransient=true;

        public virtual void SetAssignedIdTo(IdT assignedId)
        {
            if (!base.IsTransient()){
                throw new SharpArch.Domain.PreconditionException("Can't assign Id to non transient object");
            }
            this.Id = assignedId;
        }

        public virtual void OnSave()
        {
            _isTransient = false;
        }

        public virtual void OnLoad()
        {
            _isTransient = false;
        }

        public override bool IsTransient()
        {
            return _isTransient;
        }
    }

    [Serializable]
    [MappingIgnore]
    public class DomainObjectWithAssignedIntId : DomainObjectWithAssignedId<int>
    {
    }

    [Serializable]
    [MappingIgnore]
    public abstract class VersionedDomainObjectWithAssignedIntId : DomainObjectWithAssignedIntId, IVersion
    {
        public virtual int RowVersion { get; set; }
    }

    [Serializable]
    [MappingIgnore]
    public class DomainObjectWithAssignedGuidId : DomainObjectWithAssignedId<Guid>
    {
    }

    [Serializable]
    [MappingIgnore]
    public class VersionedDomainObjectWithAssignedGuidId : DomainObjectWithAssignedGuidId, IVersion
    {
        public virtual int RowVersion { get; set; }
    }
    
}
