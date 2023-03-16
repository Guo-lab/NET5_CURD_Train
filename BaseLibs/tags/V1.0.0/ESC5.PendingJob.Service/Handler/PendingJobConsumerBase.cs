using ESC5.Job.Message;
using System.Collections.Generic;
namespace ESC5.PendingJob.Service.Handler
{
    public class PendingJobConsumerBase { 
        

        private Dictionary<string,IPendingJobHandler> _handlers= new Dictionary<string,IPendingJobHandler>();
        protected IPendingJobHandler JobHandler(string OrderType)
        {
            if (!_handlers.ContainsKey(OrderType))
            {
                IPendingJobHandler handler = PendingJobHandlerFactory.CreateHandler(OrderType);
                _handlers.Add(OrderType, handler);
                
            }
            return _handlers[OrderType];
        }
        


    }
}