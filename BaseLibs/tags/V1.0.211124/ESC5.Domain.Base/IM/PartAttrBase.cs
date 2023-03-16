using ProjectBase.Domain;
namespace ESC5.Domain.Base.IM
{
    [MappingIgnore]
    public class PartAttrBase: BaseDomainObject,IPartAttribute
    {
        public virtual string PartAttrName { get; set; }
        public virtual string PartAttrValue { get; set; }
        public virtual int AttributeId { get; set; }
        public virtual bool IsFixed { get; set; }
        public virtual bool IsRequired { get; set; }

    }
    [MappingIgnore]
    public class PartAttrGuidBase : DomainObjectWithAssignedGuidId, IPartAttribute
    {
        public virtual string PartAttrName { get; set; }
        public virtual string PartAttrValue { get; set; }
        public virtual int AttributeId { get; set; }
        public virtual bool IsFixed { get; set; }
        public virtual bool IsRequired { get; set; }

    }
}
