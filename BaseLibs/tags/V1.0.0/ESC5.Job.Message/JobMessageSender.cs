using RemotingConnector;
using System;
using Microsoft.Extensions.Configuration;
using ProjectBase.MessageSender;

namespace ESC5.Job.Message
{
    public class JobMessageSender:MessageSenderBase<IJobEvent>,IMessageSender<JobOrder>
    {
        public JobMessageSender(IConfiguration config) :base(config["AppSetting:RemotingServer"], Convert.ToInt32(config["AppSetting:JobRemotingPort"]))
        {
            
        }

        public void Send(JobOrder job)
        {
            try
            {
                ConnectToServer();
                this.Proxy.Send(job);
            }catch(Exception ex)
            {
                Logger.Log.Info("Unable send job to service. Type:" + job.OrderType + ", Id:" + job.OrderID + "." + ex.Message);
            }
        }

    }
}
