using RemotingConnector;
namespace ESC5.Job.Message
{
    public interface IJobEvent
    {
        void Send(JobOrder job);
    }
}
