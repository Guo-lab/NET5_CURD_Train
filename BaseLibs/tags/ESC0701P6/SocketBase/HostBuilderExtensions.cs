using SuperSocket;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// 启用Controller架构
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static ISuperSocketHostBuilder<TextPackageInfo> UseFrontController(this ISuperSocketHostBuilder<TextPackageInfo> hostBuilder)
        {
            var fc=FrontController.Instance;
            hostBuilder.UsePackageHandler(async (s, p) =>
             {
                 await fc.HandlePackageAsync(s, p);
             }, async (s, e) =>
             {
                 return await fc.ExceptionHandler(s, e);
             });
            return hostBuilder;
        }
    }
}
