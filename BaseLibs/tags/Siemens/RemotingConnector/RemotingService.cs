using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotingConnector
{
    public class RemotingService<T>
    {
        public static Action<object, T> SendEvent;
        public void Send(T entity)
        {
            if (SendEvent != null)
            {
                SendEvent(this, entity);
            }
        }
    }
}
