using System;

namespace ESC5.AppBase.AppShutdown
{
    /// <summary>
    /// 此类实现IAppShutdownConfirmer，作用是将关停通知作为最基本的准备工作。即保证收到了通知才算准备好。
    /// </summary>
    public class SimpleFlagAppShutdownConfirmer: IAppShutdownConfirmer
    {
        private static object lockSignal = new();

        private bool shuttingFlag = false;

        public bool IsReadyToShutdown()
        {
            lock (lockSignal)
            {
                return shuttingFlag;
            }
        }

        public void OnShuttingDown()
        {
            lock(lockSignal)
            {
                shuttingFlag = true;
            }
        }

    }

}
