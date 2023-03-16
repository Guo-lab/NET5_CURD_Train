using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SuperSocket;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class Request
    {
        public string ACA 
        { 
            get 
            { 
                return Area + "." + Controller + "." + Action; 
            } 
        }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public object?[]? Parameters { get; set; }
        public Type[]? ParameterTypes { get; set; }
    }
    public class RequestContext : Request
    {
        public Type ControllerClass { get; set; }
        public BaseController ControllerInstance { get; set; }
        public MethodInfo ActionMethod { get; set; }
        public IDictionary<string, object?> Items { get; set; } = new Dictionary<string, object?>();

        public IServiceProvider RequestServices { get; set; }
    }
    public class SocketRequestContext: RequestContext
    {
        public IAppSession Session { get; set; }
        public TextPackageInfo RequestSource { get; set; }
    }
    public class SignalRRequestContext : RequestContext
    {
        public HubCallerContext HubCallerContext { get; set; }
        public HttpRequest RequestSource { get; set; }
    }
}
