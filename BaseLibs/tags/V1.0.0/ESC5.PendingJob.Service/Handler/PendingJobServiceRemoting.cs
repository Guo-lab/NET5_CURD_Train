using CoreRemoting;
using CoreRemoting.DependencyInjection;
using ESC5.Job.Message;
using System;
using System.IO;
using System.Timers;

namespace ESC5.PendingJob.Service.Handler
{
    public class PendingJobServiceRemoting
    {
        private PendingJobConsumerRemoting _consumer;
        private Timer _timer;
        private RemotingServer _remotingServer;
        private string _path = System.AppDomain.CurrentDomain.BaseDirectory + "\\JobQueue";
        public PendingJobServiceRemoting()
        {
            _consumer = new PendingJobConsumerRemoting();
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }
        public void Start()
        {
            RegisterRemoteConnection();
            RemotingJob.SendEvent += this.WriteQueue;
            _timer = new Timer(60*1000);
            _timer.Elapsed += ((sender, e) =>
            {
                _timer.Enabled = false;
                CalcPendingJob();
                _timer.Enabled = true;
            });
            _timer.Start();
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            if (_remotingServer != null)
            {
                _remotingServer.Stop();
                _remotingServer.Dispose();
            }
        }
        private void RegisterRemoteConnection()
        {
            _remotingServer = new RemotingServer(new ServerConfig()
            {
                HostName = "127.0.0.1",
                NetworkPort = AppSetting.JobRemotingPort,
                RegisterServicesAction = container =>
                {
                    container.RegisterService<IJobEvent, RemotingJob>(ServiceLifetime.Singleton);
                }
            });
            _remotingServer.Start();
        }
        
        private void WriteQueue(object sender, JobOrder order)
        {
            string fileName = _path + "\\" + order.OrderType + "-" + order.OrderID;
            if (!File.Exists(fileName))
            {
                using (var stream = File.Create(fileName))
                {
                }
            }
        }
        private void CalcPendingJob()
        {
            foreach (string file in Directory.GetFiles(_path))
            {
                string[] fileName = Path.GetFileName(file).Split('-');
                string orderType = fileName[0];
                string orderId = fileName[1];
                try
                {
                    _consumer.Consume(orderType, orderId);
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Process Joborder failed. OrderType:" + orderType + ",id:" + orderId + ex.Message);
                }
            }
        }
    }
}