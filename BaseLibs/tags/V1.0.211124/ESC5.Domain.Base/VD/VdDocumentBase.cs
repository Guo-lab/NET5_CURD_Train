using ProjectBase.Domain;
using System;

namespace ESC5.Domain.Base.VD
{
    [MappingIgnore]
 public class VdDocumentBase: BaseDomainObject
    {
        #region "persistent properties"

        public virtual string TypeName { get; set; }
        public virtual DateTime? EffectiveDateFrom { get; set; }
        public virtual DateTime? EffectiveDateTo { get; set; }
       
        #endregion
    }
}
