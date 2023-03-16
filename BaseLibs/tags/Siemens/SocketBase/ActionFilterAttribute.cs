using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class ActionFilterAttribute : Attribute, IActionFilter
    {
        public virtual int Order { get; set; }

        public virtual ControllerKindEnum ControllerKind { get; set; } = ControllerKindEnum.All;

        public virtual void OnActionExecuted(ActionFilterContext filterContext)
        {
        }

        public virtual void OnActionExecuting(ActionFilterContext filterContext)
        {

        }
    }
}
