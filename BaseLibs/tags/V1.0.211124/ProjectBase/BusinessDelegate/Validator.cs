using System.Collections.Generic;
using System.Linq;
using System;
using ProjectBase.Domain;

namespace ProjectBase.BusinessDelegate
{
    public abstract class Validator<T, TId> :IValidator<T,TId> where T : BaseDomainObjectWithTypedId<TId>
                                                                          where TId : struct
    {
        private IValidator<T,TId> NextValidator { set; get; }

        public IValidator<T,TId> SetNextValidator(IValidator<T,TId> validator)
        {
            this.NextValidator = validator;
            return validator;
        }

        public IGenericDaoWithTypedId<T,TId> Dao { get; set; }

        // 每个验证者都必须对请求做出处理
        public void HandleValidate(T item)
        {
            this.Validate(item);
            // 判断是否有下一个验证者
            if (this.NextValidator != null)
            {
                this.NextValidator.HandleValidate(item);
            }
        }
        // 每个验证者都必须对请求做出处理
        public void HandleValidateByAttribute(T item ,string attribute)
        {
            this.Validate(item);
            // 判断是否有下一个验证者
            if (this.NextValidator != null)
            {
                this.NextValidator.HandleValidateByAttribute(item, attribute);
            }
        }
        

        // 每个验证者都必须实现验证任务
        protected abstract void Validate(T order);
       
    }
}
