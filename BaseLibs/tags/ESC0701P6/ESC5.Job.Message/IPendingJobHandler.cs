namespace ESC5.Job.Message
{
    public interface IPendingJobHandler
    {
        void UpdatePendingJob(string OrderID);
    }
}
