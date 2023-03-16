using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public interface IExceptionFilter
    {
        int Order { get; }
        ControllerKindEnum ControllerKind { get;}
        void OnException(ExceptionContext filterContext);
    }
    public class ExceptionContext
    {
        public RequestContext RequestContext { get; set; }
        public Exception Exception { get; set; }
        public IActionResult Result { get; set; }
        public bool ExceptionHandled { get; set; }
    }
}
