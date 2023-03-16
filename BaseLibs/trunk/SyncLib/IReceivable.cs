using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLib
{
    public delegate void AfterReceiveHandler(IReceivable order);
    public interface IReceivable
    {
        DateTime? ReceivedTime { get; set; }
    }

    public interface IReceiveHandler
    {
        void ReceiveOrder(string orderInfo);
        //event AfterReceiveHandler AfterReceive;
    }

    public interface IReceiveHandlerFactory
    {
        IReceiveHandler CreateReceiveHandler(string handlerType);
    }
}
