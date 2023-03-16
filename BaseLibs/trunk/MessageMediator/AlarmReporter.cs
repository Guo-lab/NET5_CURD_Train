using ProjectBase.Domain;
using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator
{
    public interface IAlarmReporter:IBusinessDelegate
    {
        void Report(AlarmSeverityEnum severity, string message, Exception? e = null);
        void Info(string message, Exception? e = null);
        void Warn(string message, Exception? e = null);
        void Error(string message, Exception? e = null);

        public enum AlarmSeverityEnum
        {
            Error,
            Warn,
            Info
        }
    }
    public class AlarmReporter: IAlarmReporter
    {
        private static string fileLock = "filelock";
        public IUtil Util { get; set; }

        private DirectoryInfo alarmDir;
        public AlarmReporter()
        {
            alarmDir = Directory.CreateDirectory(GetRunningPath("AlarmReporterLog"));
        }
        public void Report(IAlarmReporter.AlarmSeverityEnum severity, string message,Exception? e=null)
        {
            Log(severity,message,e);
            //TODO:发邮件
        }
        public void Info(string message, Exception? e = null)
        {
            Report(IAlarmReporter.AlarmSeverityEnum.Info, message,e);
        }
        public void Warn(string message, Exception? e = null)
        {
            Report(IAlarmReporter.AlarmSeverityEnum.Warn, message, e);
        }
        public void Error(string message, Exception? e = null)
        {
            Report(IAlarmReporter.AlarmSeverityEnum.Error, message, e);
        }
        private void Log(IAlarmReporter.AlarmSeverityEnum severity, string message, Exception? e = null)
        {
            var path = alarmDir.FullName+"\\" + DateTime.Today.ToString("yyyy-MM-dd")+".log";
            var s = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " severity="+ severity.ToString();
            if (e != null)
            {
                message = message + "\r\n" + Util.GetExceptionMsg(e) + "\r\n"+e.StackTrace;
            }
            lock (fileLock)
            {
                File.AppendAllLines(path, new string[] { s + "    " + message + "\r\n" });
            }
        }

        private string GetRunningPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\" + relativePath;
        }
    }
}
