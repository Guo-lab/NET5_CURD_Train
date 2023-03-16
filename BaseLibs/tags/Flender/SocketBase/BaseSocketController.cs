using SuperSocket;
using SuperSocket.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class BaseSocketController: BaseController
    {
        public IAppSession Session {
            get 
            {
                return ((SocketRequestContext)Request).Session;
            }
        }

    }
}
