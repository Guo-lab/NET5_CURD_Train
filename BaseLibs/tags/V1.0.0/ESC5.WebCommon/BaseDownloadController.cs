using ESC5.AppBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectBase.BusinessDelegate;

namespace ESC5.WebCommon
{
    public class BaseDownloadController<TUser> : AppBaseController<TUser> where TUser:IFuncUser
    {
        public IApplicationStorage ApplicationStorage { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //覆盖基类方法，使得请求与_ForViewModelOnly参数无关，每个action无论是否关联视图文件，都会被执行
        }

        [HttpGet]
        public ActionResult DownloadAttachment(string psSaveTo, string psOriginalFileName, string psSavedFileName)
        {
            string sPhysicalFilePath = ApplicationStorage.GetRealPath((string.IsNullOrEmpty(psSaveTo) ? ApplicationStorage.GetAppSetting("DefaultFilePath") : psSaveTo));
            string sFullPathName = sPhysicalFilePath + "\\" + psSavedFileName;
            if (!System.IO.File.Exists(sFullPathName))
            {
                return new EmptyResult();
            }
            var stream = System.IO.File.OpenRead(sFullPathName);
            return File(stream, "application/octet-stream", psOriginalFileName);
        }
    }
}