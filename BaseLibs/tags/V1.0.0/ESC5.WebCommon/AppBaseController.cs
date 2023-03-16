using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.Web.Mvc.Angular;
using System.Net;
using IdentityService;
using ProjectBase.Domain.Transaction;
using ESC5.ValueObject;
using ESC5.AppBase;

namespace ESC5.WebCommon
{
    public class AppBaseController<TUser> : BaseController where TUser:IFuncUser
    {
        public const string Message_OperateSuccessfully = "Message_OperateSuccessfully";
        public IGenericIdentity<TUser> Identity { get; set; }
        public static string DefaultPassword = "1";//默认密码为1
        public static string AdminCode = "Admin"; //用于管理部门和用户的固定代码

        public ITransactionHelper TransactionHelper { get; set; }

        public IAttachmentHandler AttachmentHandler { get; set; }

        /// <summary>
        /// 注意，调用此方法时如果需要用到User对象的Dept,Supervisor等many-to-one属性或者Funcs，Commodities等one-to-many
        /// 属性，务必传递refresh=true再使用
        /// 如果当前用户正在代理其他用户，返回被代理的用户；如果没有代理，返回登录用户
        /// 此方法应在具有可代理的方法中调用，例如Approve, Reject。如果是操作性质的，如Publish,Close,即使正在代理也应使用登录用户而不是代理用户
        /// </summary>
        /// <returns></returns>
        protected TUser GetWorkingUser(bool refresh = false)
        {
            return Identity.GetWorkingUser(refresh)!;
        }
        /// <summary>
        /// 返回登录用户
        /// </summary>
        /// <param name="refresh"></param>
        /// <returns></returns>
        protected TUser GetLoginUser(bool refresh = false)
        {
            return Identity.GetLoginUser(refresh)!;
        }
        protected bool CanAccess(string funcCode)
        {
            int? workingUserId = Identity.GetLoginInfo()?.WorkingUserId;
            if (workingUserId == null) { return false; }
            TUser workingUser = Identity.RefreshUser(workingUserId.Value);
            return workingUser.CanAccess(funcCode);
        }

        protected void CheckFuncAuth(string funcCode)
        {
            var workingUser = Identity.GetWorkingUser(true);
            if (workingUser!.CanAccess(funcCode)) return;
            throw new AuthFailureException();
        }

        protected ActionResult FileNotFound()
        {
            return RcJsonError("FileNotFound", "FileNotFound");
        }
        public string ToHtmlEntityString(string s)
        {
            var chars = s.ToCharArray();
            var entitystring = "";
            Array.ForEach(chars, c =>
                                     {
                                         var ss = WebUtility.UrlEncode(c + "").Replace("%u", "&#x");
                                         entitystring = entitystring + ss;
                                         if (ss.StartsWith("&#x")) entitystring = entitystring + ";";
                                     });
            return entitystring;
        }

        protected string GetConvertedSortString(string orderExpression, Dictionary<string, string> fieldMapping)
        {
            string[] sort = orderExpression.Split(',');
            List<string> convertedSort = new List<string>();
            for (int i = 0; i < sort.Length; i++)
            {
                string tempField = sort[i].Split(' ')[0];
                string tempSort = sort[i].Split(' ')[1];
                if (fieldMapping.ContainsKey(tempField))
                {
                    convertedSort.Add(fieldMapping[tempField] + ' ' + tempSort);
                }
                else
                {
                    convertedSort.Add(sort[i]);
                }
            }
            return string.Join(",", convertedSort);
        }
        protected RichClientJsonResult SaveOk()
        {
            return (RichClientJsonResult)ClientShowMessage(BaseController.Message_SaveSuccessfully);
        }

        protected RichClientJsonResult OpOk()
        {
            return (RichClientJsonResult)ClientShowMessage(Message_OperateSuccessfully);
        }

        protected void MoveAttachment(AttachmentVM? attachment)
        {
            if (attachment != null)
            {
                TransactionHelper.AddSyncTask(() => AttachmentHandler.MoveToFormalFolder(attachment));
            }
        }

        protected void MoveAttachment(IEnumerable<AttachmentVM>? attachmentList)
        {
            if (attachmentList != null)
            {
                TransactionHelper.AddSyncTask(() =>
                {
                    foreach (AttachmentVM attachment in attachmentList)
                    {
                        AttachmentHandler.MoveToFormalFolder(attachment);
                    }
                });
            }
        }
    }

    public class AppBaseControllerExceptionFilter : BaseControllerExceptionFilter
    {
        public AppBaseControllerExceptionFilter(IUtil util) : base(util) { }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is AuthFailureException)
            {
                filterContext.Result = new RichClientJsonResult(false, "AuthFailure", "AuthFailure");
                filterContext.ExceptionHandled = true;
                return;
            }
            base.OnException(filterContext);
        }
    }
}
