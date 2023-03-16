using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class ActionFilterContext
    {
        public RequestContext Request { get; set; }
        public Exception? Exception { get; set; }
        public bool ExceptionHandled { get; set; } = true;
        public IActionResult? Result { get; set; }

    }
}
