using ProjectBase.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc.Validation
{
    public abstract class ValidatableObject : System.ComponentModel.DataAnnotations.IValidatableObject
    {
        /// <summary>
        /// 字段验证都通过后，执行整体验证。此方法可接受一个参数。缺省实现是调用无参的Validate方法。
        /// 子类如果需要使用参数validationContext，应重写此方法
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var msg = Validate();
            yield return msg == null ? ValidationResult.Success : new ValidationResult(msg);
        }
        /// <summary>
        /// 字段验证都通过后，执行整体验证
        /// </summary>
        /// <returns>错误消息文本。无错返回null</returns>
        public abstract string Validate();
    }
}
