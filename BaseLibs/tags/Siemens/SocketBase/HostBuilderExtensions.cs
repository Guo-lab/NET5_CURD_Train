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
                 //此处是Socket接收报文的线程起始处

                 await fc.HandlePackageAsync(s, p);
             }, async (s, e) =>
             {
                 //此处对Socket接收报文的线程进行错误捕获

                 return await fc.ExceptionHandler(s, e);
             });
            return hostBuilder;
        }
    }
}
