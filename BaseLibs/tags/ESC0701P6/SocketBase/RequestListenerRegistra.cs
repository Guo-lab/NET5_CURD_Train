using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    /// <summary>
    /// 注册RequestListener。可指定执行顺序。默认为0.
    /// </summary>
    public class RequestListenerRegistra
    {
        public SortedList<int,Action<RequestContext>> BeginRequest { get; } = new SortedList<int, Action<RequestContext>>();
        public SortedList<int,Action<RequestContext>> EndRequest { get; } = new SortedList<int, Action<RequestContext>>();


        public void AddBeginHandler(Action<RequestContext> handler,int order=0)
        {
            BeginRequest.Add(order,handler);
        }
        public void AddEndHandler(Action<RequestContext> handler, int order = 0)
        {
            EndRequest.Add(order,handler);
        }
    }
}
