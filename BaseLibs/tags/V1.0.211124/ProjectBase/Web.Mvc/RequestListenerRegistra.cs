using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharpArch.NHibernate.Web.Mvc;

namespace ProjectBase.Web.Mvc {
    public class RequestListenerRegistra
    {
        public List<Action<HttpContext>> BeginRequest { get; } = new List<Action<HttpContext>>();
        public List<Action<HttpContext>> EndRequest { get; } = new List<Action<HttpContext>>();


        public void AddBeginHandler(Action<HttpContext> handler)
        {
            BeginRequest.Add(handler);
        }
        public void AddEndHandler(Action<HttpContext> handler)
        {
            EndRequest.Add(handler);
        }
    }
}
