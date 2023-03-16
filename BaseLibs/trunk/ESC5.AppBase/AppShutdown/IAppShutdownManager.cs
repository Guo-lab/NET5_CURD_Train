using System;

namespace ESC5.AppBase.AppShutdown
{
     public interface IAppShutdownManager
    {
        void NotifyShutdown();
        bool IsReadyToShutdown();
    }
 
}
