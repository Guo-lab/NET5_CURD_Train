using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public interface IActionFilter
    {
        int Order { get; }

        /// <summary>
        /// 适用于哪种Controller
        /// </summary>
        ControllerKindEnum ControllerKind { get; }

        /// <summary>
        /// action执行前执行此方法。 有异常应抛出，不要设置filterContext.Exception。如果进入action之前就设置了Result则不会进入action
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns>是否继续执行下一个filter或action</returns>
        void OnActionExecuting(ActionFilterContext filterContext);

        /// <summary>
        /// action执行后执行此方法。 有异常可抛出，如果要继续下一个filter则不要抛异常，而可以设置filterContext.Exception，并且可以修改Result
        /// </summary>
        /// <param name="filterContext"></param>
        void OnActionExecuted(ActionFilterContext filterContext);
    }
    public enum ControllerKindEnum
    {
        All,
        Hub,
        Socket
    }
}
