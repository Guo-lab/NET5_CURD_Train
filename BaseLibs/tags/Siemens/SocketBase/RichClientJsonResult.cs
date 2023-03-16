using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class RichClientJsonResult : ActionResult
    {
        public static string Key_KeepDateFormat = "KeepDateFormat";
        public static string Key_KeepDateAsNumber = "KeepDateAsNumber";
        public static string Command_Noop = "Noop";
        public static string Command_Message = "Message";
        public static string Command_Redirect = "Redirect";
        public static string Command_AppPage = "AppPage";
        public static string Command_ServerVM = "ServerVM";
        public static string Command_ServerData = "ServerData";
        public static string Command_BizException = "BizException";
        public static string Command_Exception = "Exception";

        private object _extra;

        public object? resultdata { get; set; }
        public bool isRcResult { get; set; }
        public string command { get; set; }
        public bool IsErrorResult { get { return isRcResult == false; } }
        public object Extra { set => this._extra = value; }

        public RichClientJsonResult(bool isRcResult, string command, object? resultdata)
        {
            this.isRcResult = isRcResult;
            this.command = command;
            this.resultdata = resultdata;
        }
        public override object? ExecuteResult()
        {
            object Value = new RcResult { isRcResult= isRcResult, command= command, data = resultdata };
            return Value;
        }
    }
    public class RcResult {
        public object? data { get; set; }
        public bool isRcResult { get; set; }
        public string command { get; set; }
    }

}
