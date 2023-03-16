using ProjectBase.Utils;
using ProjectBase.Web.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBase.Web.Mvc.Angular;

namespace ESC5.WebCommon
{
    /// <summary>
    /// 因为本应用与登录无关，此处不继承ESC5.WebCommon.AppBaseController<TUser>,所以直接继承BaseController
    /// </summary>
    public class AppBaseController : BaseController 
    {
        public const string Message_OperateSuccessfully = "Message_OperateSuccessfully";
   
        protected RichClientJsonResult SaveOk()
        {
            return (RichClientJsonResult)ClientShowMessage(Message_SaveSuccessfully);
        }

        protected RichClientJsonResult OpOk()
        {
            return (RichClientJsonResult)ClientShowMessage(Message_OperateSuccessfully);
        }

    }

}
