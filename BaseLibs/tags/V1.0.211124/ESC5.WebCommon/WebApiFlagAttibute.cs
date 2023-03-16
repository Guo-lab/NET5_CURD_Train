using System;

namespace ESC5.WebCommon
{

    /**
	 * 作为webapi的action方法上用于设置webapiguard参数的标记。.
	 * {@link WebApiGuard}对每个Action方法都同样处理，有些特殊的action就需要标记出特殊的设置参数，以便{@link WebApiGuard}处理时区别对待
	 * @author Rainy
	 *
	 */
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class WebApiFlagAttribute : Attribute
    {
        /**
         * 忽略登录令牌检查
         */
        public bool IgnoreLoginToken { get; set; }
        /**
         * 忽略表单令牌检查
         */
        public bool IgnoreFormToken { get; set; }

    }
}
