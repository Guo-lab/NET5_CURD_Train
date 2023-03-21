using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System;

namespace ESC5.Domain.DomainModel.TR
{
    public class TaskItem: BaseDomainObject
    {
        #region "persistent properties"
        [DomainSignature]
        public virtual Task Task { get; set; }
        [DomainSignature]
        public virtual string ItemNo { get; set; }

        #endregion

    }
}
