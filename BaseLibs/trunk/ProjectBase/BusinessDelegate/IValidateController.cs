using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectBase.Domain;

namespace ProjectBase.BusinessDelegate
{
    public interface IValidateController<T,TId> where T : BaseDomainObjectWithTypedId<TId>
                                                                          where TId : struct
    {
        void StartValidate(T item);
        void StartValidateByAttribute(T item, string attribute);
    }
}
