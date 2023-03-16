using log4net;

namespace Logger
{
    public class Log
    {
        private static ILog log;
        private static ILog Logger {
            get
            {
                if (log == null)
                    log = LogManager.GetLogger("App");
                return log;
            }
        }
        public static void Info(string psLog)
        {
           Logger.Info(psLog);
        }

        public static void Error(string psLog)
        {
            Logger.Error(psLog);
        }
    }
}
