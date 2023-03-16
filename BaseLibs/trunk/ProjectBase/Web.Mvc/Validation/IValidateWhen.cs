using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc.Validation
{
    public interface IValidateWhen
    {
        /// <summary>
        ///  指定要验证的分组。此方法接受一个参数。缺省实现是调用无参的同名方法。
        ///  子类如果需要使用参数validationContext，应重写此方法。
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns>要验证的组名，多个组名用逗号分隔，缺省组名为 _D。返回null表示除无条件总是验证组_A外不验证任何组</returns>
        string ShouldValidateGroups(ValidationContext validationContext) {
            return ShouldValidateGroups();
        }
        /// <summary>
        /// 指定要验证的分组
        /// </summary>
        /// <returns>要验证的组名，多个组名用逗号分隔，缺省组名为 _D。返回null表示除无条件总是验证组_A外不验证任何组</returns>
        string ShouldValidateGroups();
    }
}
