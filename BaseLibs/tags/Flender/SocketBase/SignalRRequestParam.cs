using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class SignalRRequestParam
    {
        /// <summary>
        /// 请求参数中字符串指明area.controller.action
        /// </summary>
        public string ACA { get; set; }
        /// <summary>
        /// action参数名值对照，值为json字符串
        /// </summary>
        public Dictionary<string, string> ActionParamJsonMap { get; set; }
    }
}
