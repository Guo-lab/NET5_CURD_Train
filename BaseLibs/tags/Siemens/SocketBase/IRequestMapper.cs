using Microsoft.AspNetCore.SignalR;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public interface IRequestMapper
    {
        SocketRequestContext MapSocket(TextPackageInfo package);
        SignalRRequestContext MapSingalR(SignalRRequestParam param);
    }

}
