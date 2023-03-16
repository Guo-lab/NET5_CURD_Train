using System;
using System.Collections.Generic;
using System.Linq;
using SyncLib.Exchange;

namespace SyncLib
{
    public delegate void AfterDownloadHandler(ICanSynchronize order);
    public interface ICanSynchronize
    {
        DateTime? SyncTime { get; set; }
       
    }

    public interface ISynchronizeHandler
    {
        IList<Exchange.Message> GetDownloadableOrder();
        string OrderType { get; set; }
    }

    public interface ISynchronizeHandlerFactory
    {
        ISynchronizeHandler CreateSynchronizeHandler(string handlerType);
    }
}
