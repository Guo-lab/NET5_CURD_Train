using System;

namespace ESC5.AppBase.AppShutdown
{
    /// <summary>
    /// 应用关停前需要进行相关准备工作，所有Confirmer都准备好了才能关停。<br></br>
    /// 凡是在应用关停前需要做准备工作的类应实现该接口
    /// </summary>
    public interface IAppShutdownConfirmer
    {
        //确认是否完成关停准备
        bool IsReadyToShutdown();

        /// <summary>
        /// 当应用为关停做准备时
        /// </summary>
        void OnShuttingDown();

    }
}
