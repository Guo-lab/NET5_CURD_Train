using ProjectBase.BusinessDelegate;
using ProjectBase.Domain;
using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public abstract class BaseController: IController
    {
        public IConnectionManager ConnectionManager { get; set; }
        public RequestContext Request { get; set; }
        public IExceptionTranslator ExTranslator { get; set; }
        protected ValueTask SendToSocketTargetAsync(string targetId, object msg)
        {
            return ConnectionManager.SendToSocketTargetAsync(targetId,msg);
        }
        protected virtual Task SendToSingalRTargetAsync(object paramObj,Topic? topic=null ,string? targetId=null, string? clientAction=null)
        {
            return ConnectionManager.SendToSingalRTargetAsync(paramObj, topic, targetId, clientAction);
        }
        public IActionResult Task(ValueTask task)
        {
            return new ActionResult
            {
                Task= task
            };
        }
        public RichClientJsonResult RcJson()
        {
            return new RichClientJsonResult(true, RichClientJsonResult.Command_Noop, null);
        }
        public RichClientJsonResult RcJson(Object data)
        {
            return RcJson(RichClientJsonResult.Command_ServerData, data);
        }
        public RichClientJsonResult RcJson(string command, Object data)
        {
            return new RichClientJsonResult(true, command, data);
        }
        public RichClientJsonResult RcJsonError(string msg)
        {
            return RcJsonError(RichClientJsonResult.Command_Message, msg);
        }
        public RichClientJsonResult RcJsonError(string command, string msg)
        {
            return new RichClientJsonResult(false, command, msg);
        }
        public RichClientJsonResult RcJsonError(Exception ex)
        {
            if (ex is BizException bizEx)
            {
                return RcJsonError(RichClientJsonResult.Command_BizException, bizEx.ExceptionKey);
            }
            else
            {
                return RcJsonError(RichClientJsonResult.Command_Exception, ex.Message);
            }
        }
        protected ActionResult Noop()
        {
            return RcJson();
        }

        protected ActionResult AuthFailure()
        {
            return RcJsonError("AuthFailure", "AuthFailure");
        }

        /**
        * {@link #commitOnErrorResult(boolean commit)}的过载版 设置当action返回结果为ErrorResult时也提交事务
        */
        protected void CommitOnErrorResult()
        {
            CommitOnErrorResult(true);
        }

        /**
         * 设置当action返回结果为ErrorResult时是否也提交事务
         * 
         * @param commit 是否提交
         */
        protected void CommitOnErrorResult(bool commit)
        {
            Request.Items[TransactionAttribute.RequestAttr_CommitOnErrorResult] = commit ? true : null;
        }
        protected void Ensure(bool assertion, string errMsgKey)
        {
            if (!assertion) throw new PreHandleResultException(RcJsonError(errMsgKey));
        }
    }

     public abstract class BaseControllerExceptionFilter : IExceptionFilter
    {
        public static readonly string KEY_EXCEPTION_LOGGED = "NetArch_Exception_Logged_By_BaseControllerExceptionFilter";
        private IUtil Util { get; set; }

        public abstract ControllerKindEnum ControllerKind { get; }

        public int Order => 0;

        public BaseControllerExceptionFilter(IUtil util)
        {
            Util = util;
        }
        public virtual void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception is ActionInvocationException ? filterContext.Exception.InnerException! : filterContext.Exception;
            if (ex is PreHandleResultException)
            {
                filterContext.Result = ((PreHandleResultException)ex).FailureResult;
            }
            else if (ex is PostHandleResultException)
            {
                filterContext.Result = ((PostHandleResultException)ex).FailureResult;
            }
            else if (ex is BizException bizEx)
            {
                filterContext.Result = new RichClientJsonResult(false, RichClientJsonResult.Command_BizException, bizEx.ExceptionKey);
            }
            else
            {
                var allMsg = "";
                RecursiveMessage(ex, ref allMsg);
                Util.AddLog("BaseController.OnException", ex);
                MarkExceptionAsLogged(filterContext,allMsg);
                if (Util.IsDevelopment()) return;
                filterContext.Result = new RichClientJsonResult(false, RichClientJsonResult.Command_Message, "系统运行错误，请联系管理员");
            }
            filterContext.ExceptionHandled = true;
        }
        protected void MarkExceptionAsLogged(ExceptionContext filterContext,string allMsg)
        {
            filterContext.RequestContext.Items[KEY_EXCEPTION_LOGGED] = allMsg;
        }
        protected string? GetLoggedMessage(ExceptionContext filterContext, string allMsg)
        {
            if (!filterContext.RequestContext.Items.ContainsKey(KEY_EXCEPTION_LOGGED)) return null;
            return filterContext.RequestContext.Items[KEY_EXCEPTION_LOGGED]!.ToString();
        }
        protected void RecursiveMessage(Exception e, ref string allMsg)
        {
            allMsg += e.Message;
            if (e.InnerException != null)
            {
                allMsg += "\r\n";
                RecursiveMessage(e.InnerException, ref allMsg);
            }
        }
    }
    public class PreHandleResultException : Exception
    {
        public IActionResult FailureResult { get; set; }
        public PreHandleResultException() : base() { }
        public PreHandleResultException(IActionResult failureResult)
        {
            FailureResult = failureResult;
        }
    }
    public class PostHandleResultException : Exception
    {
        public IActionResult FailureResult { get; set; }
        public PostHandleResultException() : base() { }
        public PostHandleResultException(IActionResult failureResult)
        {
            FailureResult = failureResult;
        }
    }
}
