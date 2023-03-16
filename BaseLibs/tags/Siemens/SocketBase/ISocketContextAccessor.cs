using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public interface ISocketContextAccessor
    {
        public RequestContext? Request { get; }
    }
    public class SocketContextAccessor : ISocketContextAccessor
    {
        public RequestContext? Request
        {
            get
            {
                return FrontController.RequestForActionThread?.Value;
            }
        }
    }
}
