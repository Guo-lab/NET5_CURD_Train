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
using NHibernate.Exceptions;
using ZXing;
using Org.BouncyCastle.Asn1.Ocsp;
using SharpArch.Domain;

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

        protected JsonResult AjaxJson(object model, bool genFormToken = true)
        {
            dynamic vm = new
            {
                ViewModel = model,
                ViewModelTypeName = model == null ? "ViewModel instance should not be null" : model.GetType().Name,
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
        protected ActionResult ForView(object model, bool genFormToken = true)
        {
            return ForView(null, model, genFormToken);
        }
        [Obsolete("不再支持，vm对象不可为空，应改用ForView(string viewName, object model, bool genFormToken = true)")]
        protected ActionResult ForView(string viewName)
        {
            return ForView(viewName, null);
        }

        protected ActionResult ForView(string viewName, object model, bool genFormToken = true)
        {
           // Check.Require(model != null, "model不可为null"); 暂不强制，由客户端报错提醒
            if (Request.Query.ContainsKey(GlobalConstant.Key_For_ForViewModelOnly) && Request.Query[GlobalConstant.Key_For_ForViewModelOnly].Single() == "true")
            {
                _viewMode = model;
                OnViewExecuting(model);
                return AjaxJson(model, genFormToken);
            }
            return View(viewName, model);
        }

        /*
         * Mapper.CreateMap方法已经不再支持，因此无法直接调用如下两个方法.调用之前需要
         * 在VM文件中创建Profile的子类并实现CreateMap(适用于较复杂的映射)
         * 或者在VM中用标记定义Domain和VM的映射(适用于Domain和VM的属性可直接对应或简单的计算)
         * */
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
        public IList<TDestination> MapList<TSource, TDestination>(IList<TSource> source)
        {
            return Mapper.Map<IList<TSource>, IList<TDestination>>(source);
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

    }

    public class BaseControllerExceptionFilter : IExceptionFilter
    {
        private IUtil Util { get; set; }
        public BaseControllerExceptionFilter(IUtil util)
        {
            Util = util;
        }
        public virtual void OnException(ExceptionContext filterContext)
        {
            Util.AddLog("BaseController.OnException", filterContext.Exception);
            if (Util.IsDevelopment()) return;
            var e = filterContext.Exception.InnerException ?? filterContext.Exception;
            filterContext.Result = new RichClientJsonResult(false, RichClientJsonResult.Command_Message, e.GetType().Name);
            filterContext.ExceptionHandled = true;
        }
    }
}

