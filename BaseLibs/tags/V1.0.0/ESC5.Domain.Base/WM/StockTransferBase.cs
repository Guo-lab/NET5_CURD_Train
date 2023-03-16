using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using eBPM.Role;
using System.Collections.Generic;

namespace ESC5.Domain.Base.WM
{
    [MappingIgnore]
    public abstract class StockTransferBase : BaseDomainObject
    {

        #region "persistent properties"
        public virtual string StockTransferNo { get; set; }
        #endregion


        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }
}

