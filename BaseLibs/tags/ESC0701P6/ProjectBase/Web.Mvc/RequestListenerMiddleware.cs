using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharpArch.NHibernate.Web.Mvc;

namespace ProjectBase.Web.Mvc {
    public class RequestListenerMiddleware {
        private readonly RequestDelegate _next;

        public RequestListenerMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context, RequestListenerRegistra registra) {
            // Do tasks before other middleware here, aka 'BeginRequest'
            foreach (var handler in registra.BeginRequest.Values)
            {
                handler.Invoke(context);
            }

            // Let the middleware pipeline run
            await _next(context);

            // Do tasks after middleware here, aka 'EndRequest'
            foreach (var handler in registra.EndRequest.Values)
            {
                handler.Invoke(context);
            }
        }
    }
 
}
