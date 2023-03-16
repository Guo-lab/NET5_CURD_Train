using HttpLibrary;

namespace ESC5.PendingJob.Service.Handler
{
    public class BaseJobHandler
    {
        protected void RunTask(string url, object parameter)
        {
            HttpTransfer.Open(AppSetting.WebUrl + url, parameter);
        }
    }
}
