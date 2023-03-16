using System.Threading.Tasks;
namespace ESC5.PendingJob.Service.Handler
{
    public class PendingJobConsumerRemoting : PendingJobConsumerBase
    {
        public void Consume(string orderType, string orderId)
        {
            this.JobHandler(orderType).UpdatePendingJob(orderId);
        }
    }
}