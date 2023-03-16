using System;
using RemotingConnector;

namespace ESC5.Job.Message
{
    public class RemotingJob : RemotingService<JobOrder>,IJobEvent
    {
    }
}
