using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBase.Utils;
using ProjectBase.Web.Mvc.Angular;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using ProjectBase.Domain;
using AutoMapper;
using ProjectBase.AutoMapper;
using ProjectBase.BusinessDelegate;

namespace ProjectBase.Web.Mvc
{
    public class BaseController : Controller
    {
        protected static Exception LocateViewOnly = new Exception("此Action只是定位cshtml文件所需，不应被执行");
        private object _viewMode;

        public IUtil Util { get; set; }
        public IMapper Mapper { get; set; }
        public IExceptionTranslator ExTranslator { get; set; }

        public IFormTokenGenerator FormTokenGen { get; set; }
        protected ISession Session
        {
            get
            {
                return HttpContext.Session;
            }
        }

        /// <summary>
        ///  该方法在进入Action方法前执行，基类默认根据_ForViewModelOnly参数来决定是否进入Action。
        ///  子类Controller中，有些Action需要保证进入执行，则可覆盖重写此方法。如CommonController.NgControllerJs。
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (UrlHelperExtension.ApplicationPath == null)
            {
                UrlHelperExtension.ApplicationPath = filterContext.HttpContext.Request.PathBase;
            }
            if (Request.Query.ContainsKey(GlobalConstant.Key_For_ForViewModelOnly)) return;

            var viewname = ((ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName;
            if (Request.Query.ContainsKey(GlobalConstant.Key_For_ForceViewName))
            {
                viewname = Request.Query[GlobalConstant.Key_For_ForceViewName];
            }
            if (viewname.StartsWith("/"))
            {
                viewname = "~" + viewname;
            }
            filterContext.Result = View(viewname);
        }
        /// <summary>
        /// 该方法在Action方法后执行，用于给ViewModel的某些属性赋值。通常ViewModel的赋值在Action中完成，
        /// 只在个别情况下，比如多个action中进行同样的赋值，则可将相同的赋值语句写到此方法中。
        /// </summary>
        /// <param name="viewModel"></param>
        protected virtual void OnViewExecuting(object viewModel)
        {
        }

        protected void SetViewModel<TParam>(Action<TParam> func)
        {
            if (_viewMode is TParam && _viewMode != null)
            {
                func((TParam)_viewMode);
            }
        }

        public override ViewResult View(string viewName, object model)
        {
            _viewMode = model;
            OnViewExecuting(_viewMode);
            return base.View(viewName, model);
        }
        //protected JsonResult JsonGet(Object data)
        //{
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        protected JsonResult AjaxJson(object model, bool genFormToken = true,bool allowAnonymousModelClass=false)
        {
            var modelType = model.GetType();
            var modelTypeName = model == null ? "ViewModel instance should not be null" : modelType.Name;
            if (!allowAnonymousModelClass && (modelTypeName.StartsWith("<>f__AnonymousType") || modelType.IsPrimitive))
            {
                throw new Exception("ForwView不能使用简单类或匿名类");
            }
            dynamic vm = new
            {
                ViewModel = model,
                ViewModelTypeName = modelTypeName,
                ViewModelFormToken = model == null || !genFormToken ? null : FormTokenGen?.CreateFormToken(Request, model.GetType()),
            };
            return RcJson(RichClientJsonResult.Command_ServerVM, vm);
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
        protected ActionResult ForView(object model, bool genFormToken = true)
        {
            return ForViewWithViewNameInternal(null, model, genFormToken);
        }
        [Obsolete("仅用于兼容小房子。")]
        protected ActionResult ForViewWithAnonymousVMClass(object model, bool genFormToken = true)
        {
            return ForViewWithViewNameInternal(null, model, genFormToken,true);
        }
        [Obsolete("不再支持，vm对象不可为空，应改用ForView(object model, bool genFormToken = true)")]
        protected ActionResult ForView(string viewName)
        {
            return ForView(viewName, null);
        }
        [Obsolete("不再支持，viewName参数很少需要，如确实必要，可改用ForViewWithViewName(string viewName, object model, bool genFormToken = true)")]
        protected ActionResult ForView(string viewName, object model, bool genFormToken = true)
        {
            return ForViewWithViewNameInternal(viewName, model, genFormToken);
        }
        /// <summary>
        /// 应用层使用此方法时应注意，只在Action名与页面文件名不一致时才使用此方法
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <param name="genFormToken"></param>
        /// <returns></returns>
        [Obsolete("仅特殊情况下使用。使用时须经审批。")]
        protected ActionResult ForViewWithViewName(string viewName, object model, bool genFormToken = true, bool allowAnonymousModelClass = false)
        {
            return ForViewWithViewNameInternal(viewName, model, genFormToken, allowAnonymousModelClass);
        }
        private ActionResult ForViewWithViewNameInternal(string viewName, object model, bool genFormToken = true, bool allowAnonymousModelClass = false)
        {
            // Check.Require(model != null, "model不可为null"); 暂不强制，由客户端报错提醒
            if (Request.Query.ContainsKey(GlobalConstant.Key_For_ForViewModelOnly) && Request.Query[GlobalConstant.Key_For_ForViewModelOnly].Single() == "true")
            {
                _viewMode = model;
                OnViewExecuting(model);
                return AjaxJson(model, genFormToken, allowAnonymousModelClass);
            }
            return View(viewName, model);
        }
        //返回列表数据及其分页数据
        protected ActionResult ForList(object list, Pager pager)
        {
            return RcJson(new
            {
                ResultList = list,
                Input = new
                {
                    ListInput = new
                    {
                        Pager = pager
                    }
                }
            });
        }

        /*
         * Mapper.CreateMap方法已经不再支持，因此无法直接调用Map方法.调用之前需要
         * 在VM文件中创建Profile的子类并实现CreateMap
         * 此方法已过期，只为兼容性而保留
         * */
        [Obsolete("考虑使用GetDto直接返回DTO对象,或将TDestination定义为ISelfMapper后调用重载的Map方法")]
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, bool newDestinationObj = true)
            where TDestination : ISelfMapper<TSource>
        {
            if (newDestinationObj)
            {
                destination = Mapper.Map<TSource, TDestination>(source);
            }
            else
            {
                destination = Mapper.Map(source, destination);
            }

            destination.Map(source);

            return destination;
        }
        public TDestination Map<TSource, TDestination, TContext>(TSource source, TDestination destination, TContext context, bool newDestinationObj = true)
            where TDestination : ISelfMapper<TSource,TContext>
            where TContext : BaseSelfMapperContext
        {
            if (newDestinationObj)
            {
                destination = Mapper.Map<TSource, TDestination>(source);
            }
            else
            {
                destination = Mapper.Map(source, destination);
            }
            destination.Map(source,context);
            return destination;
        }

        /*
         * Mapper.CreateMap方法已经不再支持，因此无法直接调用MapList方法.调用之前需要
         * 在VM文件中创建Profile的子类并实现CreateMap
         * 此方法已过期，只为兼容性而保留
         * */
        [Obsolete("考虑使用GetDtoList直接返回结果集,或将TDestination定义为ISelfMapper后调用重载的MapList方法")]
        public IList<TDestination> MapList<TSource, TDestination>(IList<TSource> source)
        {
            return Mapper.Map<IList<TSource>, IList<TDestination>>(source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="newDestinationObj"></param> 是否new一个目标对象。缺省为true。如果为false，则映射到传入对象上
        /// <returns></returns>


        public IList<TDestination> MapList<TSource, TDestination>(IList<TSource> sourceList, IList<TDestination> destinationList, bool newDestinationObj = true)
                where TDestination : ISelfMapper<TSource>
        {
            if (newDestinationObj)
            {
                destinationList = Mapper.Map<IList<TSource>, IList<TDestination>>(sourceList);
            }
            else
            {
                destinationList = Mapper.Map(sourceList, destinationList);
            }
            for (int i = 0; i < destinationList.Count; i++)
            {
                destinationList[i].Map(sourceList[i]);
            }
            return destinationList;
        }
        public IList<TDestination> MapList<TSource, TDestination, TContext>(IList<TSource> sourceList, IList<TDestination> destinationList, TContext context, bool newDestinationObj = true)
                where TDestination : ISelfMapper<TSource,TContext>
                where TContext : BaseSelfMapperContext
        {
            if (newDestinationObj)
            {
                destinationList = Mapper.Map<IList<TSource>, IList<TDestination>>(sourceList);
            }
            else
            {
                destinationList = Mapper.Map(sourceList, destinationList);
            }

            for (int i = 0; i < destinationList.Count; i++)
            {
                destinationList[i].Map(sourceList[i], context);
            }
            return destinationList;
        }

        public static readonly string Message_UserInputError = "UserInput_NotPassValidation";

        public static readonly string Message_SaveSuccessfully = "Save_Successfully";

        public void SetViewMessage(string message)
        {

            ViewBag.Message = message;
        }

        private ActionResult ClientCloseFragment()
        {
            return RcJson("CloseFragment", null);
        }

        private ActionResult ClientCloseWindow()
        {
            return RcJson("CloseWindow", null);
        }
        /**
         * 给客户端的redirect命令
         * @param target:根据客户端情况协议的命令内容，对angular一般是state名，通常与action同名
         * @return
         */
        public ActionResult ClientRedirect(String target)
        {
            return ClientRedirectToAction(target);
        }

        private ActionResult ClientRedirectToAction(string action, string controller = null)
        {
            var url = Url.Action(action, controller);
            url = UrlHelperExtension.Deprefix(url);
            return RcJson(RichClientJsonResult.Command_Redirect, url);
        }
        public ActionResult ClientReloadApp(string action, string controller)
        {
            var url = Url.Action(action, controller);
            return RcJson(RichClientJsonResult.Command_AppPage, url);
        }
        public ActionResult ClientShowMessage(string message = null)
        {
            string errmsg = "";
            var modelStates = ModelState.Values;
            foreach (var modelState in modelStates)
            {
                foreach (var modelError in modelState.Errors)
                {
                    string errorText = modelError.ErrorMessage;
                    if (string.IsNullOrEmpty(errorText) && modelError.Exception != null)
                    {
                        errorText = modelError.Exception.InnerException == null ?
                            modelError.Exception.Message :
                            modelError.Exception.InnerException.Message;
                    }
                    errmsg = "\r\n" + errmsg + errorText;
                }
            }
            return errmsg == "" ? RcJson(RichClientJsonResult.Command_Message, message ?? ViewBag.Message) : RcJsonError(errmsg);
        }

        protected ActionResult Noop()
        {
            return RcJson();
        }

        protected ActionResult AuthFailure()
        {
            return RcJsonError("AuthFailure", "AuthFailure");
        }
        public ActionResult ViewAsExcel(ViewResult view)
        {
            Response.ContentType = "application/vnd.ms-excel";
            return view;
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
            HttpContext.Items[TransactionAttribute.RequestAttr_CommitOnErrorResult] = commit ? true : null;
        }
        public ActionResult JavaScript(string script)
        {
            return new JavaScriptResult(script);
        }
        /**
 * 无论客户端是否进行了验证，在服务端都必须一一验证，除了在databinding、bean validation、method validation阶段进行的验证外，在action 方法中可能还需要进行某种验证，这时使用本方法，如果验证失败则会自动中止action执行并向客户端返回错误消息，验证通过则继续执行action。
 * 
 * @param assertion
 * @param errMsgKey
 */
        protected void Ensure(bool assertion, string errMsgKey)
        {
            if (!assertion) throw new PreHandleResultException(RcJsonError(errMsgKey));

            //TODO:if (Request.HttpContext.Items.ContainsKey(GlobalConstant.Attr_Ensure))
            //{
            //    var ensureKeys = Request.HttpContext.Items[GlobalConstant.Attr_Ensure].ToString();
            //    if (string.IsNullOrEmpty(ensureKeys)) return;
            //    ensureKeys = Regex.Replace(ensureKeys, "," + errMsgKey + ",", "");
            //}

        }

    }

    public class BaseControllerExceptionFilter : IExceptionFilter
    {
        public static readonly string KEY_EXCEPTION_LOGGED = "NetArch_Exception_Logged_By_BaseControllerExceptionFilter";
        private IUtil Util { get; set; }
        public BaseControllerExceptionFilter(IUtil util)
        {
            Util = util;
        }
        public virtual void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is PreHandleResultException)
            {
                filterContext.Result = ((PreHandleResultException)filterContext.Exception).FailureResult;
            }
            else if (filterContext.Exception is BizException bizEx)
            {
                filterContext.Result = new RichClientJsonResult(false, RichClientJsonResult.Command_BizException, bizEx.ExceptionKey);
            }
            else
            {
                var allMsg = "";
                RecursiveMessage(filterContext.Exception, ref allMsg);
                Util.AddLog("BaseController.OnException", filterContext.Exception);
                filterContext.HttpContext.Items[KEY_EXCEPTION_LOGGED] = allMsg;
                if (Util.IsDevelopment()) return;
                filterContext.Result = new RichClientJsonResult(false, RichClientJsonResult.Command_Message, "系统运行错误，请联系管理员");
            }
            filterContext.ExceptionHandled = true;
        }
        private void RecursiveMessage(Exception e, ref string allMsg)
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
}

