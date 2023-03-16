using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectBase.Web.Mvc
{
    //响应回到客户端后，原页面清除后执行javascript
   public class ClientScriptResult : ActionResult
    {

        public string Script
        {
            get;
            set;
        }
        public ClientScriptResult(string script)
        {
            Script = "<script type='text/javascript' language='javascript'>" + script + "</script>";
        }
        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;

            if (Script != null)
            {
                response.WriteAsync(Script);
            }
        }
    }
}

