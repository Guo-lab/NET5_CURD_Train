using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharpArch.NHibernate.Web.Mvc;

namespace ProjectBase.Web.Mvc {

    /// <summary>
    /// 注册RequestListener。可指定执行顺序。默认为0.
    /// </summary>
    public class RequestListenerRegistra
    {
        public SortedList<int, Action<HttpContext>> BeginRequest { get; } = new SortedList<int, Action<HttpContext>>();
        public SortedList<int, Action<HttpContext>> EndRequest { get; } = new SortedList<int, Action<HttpContext>>();


        public void AddBeginHandler(Action<HttpContext> handler, int order = 0)
        {
            BeginRequest.Add(order,handler);
        }
        public void AddEndHandler(Action<HttpContext> handler, int order = 0)
        {
            EndRequest.Add(order,handler);
        }
    }
}
