using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.ByCode.Conformist;
using ProjectBase.Domain;

namespace ProjectBase.Domain.NhibernateMapByCode
{
    /// <summary>
    /// all classes implementing this interface will be have a "RowVersion" field.
    /// </summary>
    public interface IVersion
    {
        int RowVersion { get; set; }
    }
}
