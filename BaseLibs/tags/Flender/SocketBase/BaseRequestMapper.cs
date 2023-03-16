using ProjectBase.Utils;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class BaseRequestMapper : IRequestMapper
    {
        public virtual SocketRequestContext MapSocket(TextPackageInfo package)
        {
            var singleParamObj = MapSingleParamForSocket(package);
            var request = CreateSocketRequest();
            request.Area = FrontController.DEFAULT_AREA;
            request.Parameters = new[] { singleParamObj };
            request.ParameterTypes = singleParamObj == null ? null : new[] { singleParamObj.GetType() };
            return request;
        }
        protected virtual object? MapSingleParamForSocket(TextPackageInfo package)
        {
            return "";
        }
        protected virtual SocketRequestContext CreateSocketRequest()
        {
            return new SocketRequestContext();
        }
        public virtual SignalRRequestContext MapSingalR(SignalRRequestParam param)
        {
            var aca = param.ACA.ToString().Split(".");
            string controller = "";
            string action = "";
            string area = FrontController.DEFAULT_AREA;
            if (aca.Length == 2)
            {
                controller = aca[0];
                action = aca[1];
            }
            else if (aca.Length == 3)
            {
                area = aca[0];
                controller = aca[1];
                action = aca[2];
            }
            else
            {
                throw new NetArchException("SignalR Url参数ACA格式错误");
            }
            var request= new SignalRRequestContext()
            {
                Area= area,
                Controller = controller,
                Action = action
            };
            return request;
        }
        protected virtual SignalRRequestContext CreateSignalRRequest()
        {
            return new SignalRRequestContext();
        }
    }
}
