using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Application;

namespace ProjectBase.BusinessDelegate
{
    public class ValidateController<T, TId> : IValidateController<T, TId> where T : BaseDomainObjectWithTypedId<TId>
                                                                          where TId : struct
    {



        public ValidateController()
        {

            //_validator = CreateValidatorChain();
        }

        private IValidator<T, TId> CreateValidatorChain(string attribute)
        {
            var tempValidators = CastleContainer.WindsorContainer.ResolveAll<IValidator<T, TId>>();
            var validators = tempValidators.Where(x =>
            {
                ValidatorAttribute type = (ValidatorAttribute)Attribute.GetCustomAttribute(x.GetType(), typeof(ValidatorAttribute));
                if (string.IsNullOrEmpty(attribute))
                {
                    return type == null;
                }
                else
                {
                    if (type != null)
                    {
                        return false;
                    }
                    else
                    {
                        return type.attribute.Split(',').Contains(attribute);
                    }
                }


            }).ToList();

            if (validators.Count >= 1)
            {
                var validator = validators[0];
                for (int i = 1; i < validators.Count; i++)
                {
                    validator.SetNextValidator(validators[i]);
                    validator = validators[i];
                }
                return validators[0];
            }
            return null;
        }

    public void StartValidate(T item)
    {
        var _validator = CreateValidatorChain(null);
        if (_validator != null)
        {
            _validator.HandleValidate(item);
        }
    }
    public void StartValidateByAttribute(T item, string attribute)
    {
        var _validator = CreateValidatorChain(attribute);
        if (_validator != null)
        {
            _validator.HandleValidateByAttribute(item, attribute);
        }
    }

}
}
