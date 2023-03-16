using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public class ClientServerConst
    {
        public static string CLIENT_PROMISE_MANAGER_CALL_BACK_METHOD = "ClientPromiseManager_CallbackOnServerComplete";
        public static string CLIENT_PROMISE_SERVER_RETURN_STATUS_RESOLVED = "Resolved";
        public static string CLIENT_PROMISE_SERVER_RETURN_STATUS_REJECTED = "Rejected";

        public static string CLIENT_CALLBACK_ON_SERVER_RETURN = "OnServerReturn";
        public static string CLIENT_CALLBACK_ON_SERVER_ERROR = "OnServerError";
    }
}
