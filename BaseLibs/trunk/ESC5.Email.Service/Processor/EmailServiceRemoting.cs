using CoreRemoting;
using CoreRemoting.DependencyInjection;
using ESC5.Email.Message;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Timers;

namespace ESC5.Email.Service.Processor
{
    public class EmailServiceRemoting
    {
        private string _path = System.AppDomain.CurrentDomain.BaseDirectory + "\\EmailQueue";
        private Timer _timer;
        private RemotingServer _remotingServer;
        private EmailConsumerRemoting _consumer;
        public EmailServiceRemoting()
        {
            _consumer = new EmailConsumerRemoting();
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        public void Start()
        {
            RegisterRemoteConnection();
            RemotingEmail.SendEvent += this.EmailReceived;
            _timer = new Timer(60 * 1000);
            _timer.Elapsed += ((sender, e) =>
            {
                _timer.Enabled = false;
                SendEmail();
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
                NetworkPort = AppSetting.EmailRemotingPort,
                RegisterServicesAction = container =>
                {
                    container.RegisterService<IEmailEvent, RemotingEmail>(ServiceLifetime.Singleton);
                }
            });
            _remotingServer.Start();
        }

        private void EmailReceived(object sender, EmailMessage email)
        {
            string fileName = _path + "\\" + Guid.NewGuid().ToString() + ".txt";

            using (var stream = File.Create(fileName))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(JsonConvert.SerializeObject(email));
                    writer.Close();
                }
                stream.Close();
            }
        }

        private void SendEmail()
        {
            foreach (string fileName in Directory.GetFiles(_path))
            {
                EmailMessage email = JsonConvert.DeserializeObject<EmailMessage>(File.ReadAllText(fileName));
                try
                {
                    _consumer.Consume(email);
                    MoveFile(fileName, "Succeed");
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex.Message);
                    MoveFile(fileName, "Failed");
                }
            }
        }

        private void MoveFile(string sourceFile, string targetFolder)
        {
            string folder = _path + "\\" + targetFolder;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            folder = folder + "\\" + DateTime.Today.ToString("yyyyMMdd");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            File.Move(sourceFile, folder + Path.GetFileName(sourceFile));
        }
    }
}
