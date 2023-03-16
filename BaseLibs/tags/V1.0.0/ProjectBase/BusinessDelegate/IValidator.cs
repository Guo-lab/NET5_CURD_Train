using System.Collections.Generic;
using ProjectBase.Domain;
namespace ProjectBase.BusinessDelegate
{
    public interface IValidator<T,TId> where T : BaseDomainObjectWithTypedId<TId>
                                                                          where TId : struct
    {
        void HandleValidate(T item);
        IValidator<T,TId> SetNextValidator(IValidator<T,TId> validator);
        void HandleValidateByAttribute(T item, string attribute);

    }
}