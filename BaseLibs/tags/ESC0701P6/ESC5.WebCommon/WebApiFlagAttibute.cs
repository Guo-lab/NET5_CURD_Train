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

        public static readonly int DEFAULT_DELAY = 10; 

        private int _delayRequest= DEFAULT_DELAY;//缺省10s内不能重复提交，如设置为负数表示允许重复提交
        public int DelayRequest
        {
            get {
                return _delayRequest;
            }
            set {
                _delayRequest = value;
            }
        }

    }
}
