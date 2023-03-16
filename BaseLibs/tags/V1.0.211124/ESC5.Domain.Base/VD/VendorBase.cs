using eBPM.Role;
using ESC5.ValueObject;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ESC5.Domain.Base.VD
{
    [MappingIgnore]
    public abstract class VendorBase : BaseDomainObject, IVendor
    {
        public virtual string E2bizCode { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        public virtual string RegistrationCode { get; set; }

        public virtual int VendorStatus { get; set; }
        public virtual DateTime CreatedTime { get; set; }
        public virtual DateTime? ApprovedTime { get; set; }

        public virtual DateTime? SyncTime { get; set; }
        public virtual DateTime? ReceivedTime { get; set; }

        public abstract bool InEvaluation();
        public abstract bool Certificated();
    }
    [MappingIgnore]
    public abstract class VendorBase<ContactT> : VendorBase where ContactT:IVendorContact 
    {
        #region "persistent properties"
        public virtual IList<ContactT> Contacts { get; set; }

        [MappingIgnore]
        public virtual string Email
        {
            get
            {
                return string.Join(",", this.Contacts.Select(x => x.Email).Distinct());
            }
        }

        [MappingIgnore]
        public virtual string ContactName
        {
            get
            {
                return string.Join(",", this.Contacts.Select(x => x.ContactName).Distinct());
            }
        }
        #endregion
    }
}
