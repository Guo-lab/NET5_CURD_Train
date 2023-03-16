using System;
using System.Collections.Generic;
using System.Linq;
using ESC5.Domain.Base.Exception;
using ESC5.ValueObject;
using ProjectBase.Domain;

namespace ESC5.Domain.Base.IM
{
    [MappingIgnore]
    public class PartUnitBase: DomainObjectWithUniqueId
    {
        public virtual string Unit { get; set; }
        public virtual bool IsPrimary { get; set; }
        public virtual decimal Rate { get; set; }
        public virtual IPart Part { get; set; }
    }

    public static class PartUnitExt
    {
        public static UnitVO GetUnitVO(this IEnumerable<PartUnitBase> unitList, string unitCode)
        {
            PartUnitBase? unit = unitList.FirstOrDefault(x => x.Unit.ToUpper()==unitCode.ToUpper());
            if (unit == null)
            {
                throw new UnitNotDefinedException();
            }
            PartUnitBase primaryUnit = unitList.First(x => x.IsPrimary);
            return new UnitVO(unitCode, primaryUnit.Unit, unit.Rate);
        }

        
    }
}
