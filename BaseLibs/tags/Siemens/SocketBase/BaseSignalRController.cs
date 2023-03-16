using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SuperSocket;
using SuperSocket.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class BaseSignalRController: BaseController
    {
        public HubCallerContext CallerContext
        {
            get
            {
                return ((SignalRRequestContext)Request).HubCallerContext;
            }
        }
        public Topic? Topic
        {
            get
            {
                var request = (SignalRRequestContext)Request;
                if (request.HubCallerContext.Items.ContainsKey(BaseConnectionManager.KEY_SIGNALR_TOPIC))
                {
                    return (Topic?)request.HubCallerContext.Items[BaseConnectionManager.KEY_SIGNALR_TOPIC];
                }
                return null;
            }
        }

    }

}
