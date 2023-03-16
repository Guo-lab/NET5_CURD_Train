using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using ProjectBase.BusinessDelegate;
using ESC5.AppBase;
using ESC5.ValueObject;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESC5.WebCommon
{
    public class FileUploadResult
    {
        /// <summary>
        /// 文件的逻辑名称
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 文件的物理名称
        /// </summary>
        public string SavedFileName { get; set; }


        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    public abstract class UploadController<TUser> : AppBaseController<TUser> where TUser:IFuncUser
    {
        public IWebHostEnvironment Env { get; set; }
        public IApplicationStorage AppState { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //覆盖基类方法，使得请求与_ForViewModelOnly参数无关，每个action无论是否关联视图文件，都会被执行
        }
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadAttachment(string SaveTo)
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
            {
                return Content("");
            }
            var tempFilePath = AppState.GetAppSetting("DefaultFilePath");
            if (string.IsNullOrEmpty(SaveTo))
            {
                SaveTo = tempFilePath;
            }

            FileUploadResult uploadResult = Upload(Request.Form.Files[0], SaveTo);

            if (string.IsNullOrEmpty(uploadResult.ErrorMessage))
            {
                //string sUrl = Url.Action("DownloadAttachment", "Download", new {psSaveTo=SaveTo, psOriginalFileName = oFileUploadResult.OriginalFileName, psSavedFileName = oFileUploadResult.SavedFileName });
                return Content(JsonConvert.SerializeObject(new AttachmentVM(SaveTo)
                {
                    OriginalFileName = uploadResult.OriginalFileName,
                    SavedFileName = uploadResult.SavedFileName,
                    Temporary = SaveTo == tempFilePath
                }));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { error = uploadResult.ErrorMessage }));
            }
        }

        [HttpPost]
        public ActionResult UploadAttachment_CheckLogin(string SaveTo)
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
            {
                return Content("");
            }
            if (string.IsNullOrEmpty(SaveTo))
            {
                SaveTo = AppState.GetAppSetting("DefaultFilePath");
            }
            var loginUser = this.GetLoginUser();
            if (loginUser == null)
            {
                return Content(JsonConvert.SerializeObject(new { error = "登录超时，请重新登录" }));
            }
            FileUploadResult fileUploadResult = Upload(Request.Form.Files[0], SaveTo);

            if (string.IsNullOrEmpty(fileUploadResult.ErrorMessage))
            {
                string url = Url.Action("DownloadAttachment", "Download", new { psSaveTo = SaveTo, psOriginalFileName = fileUploadResult.OriginalFileName, psSavedFileName = fileUploadResult.SavedFileName });
                return Content(JsonConvert.SerializeObject(new { url = url, Uploader = GetWorkingUserName(loginUser), SavedFileName = fileUploadResult.SavedFileName }));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new { error = fileUploadResult.ErrorMessage }));
            }
        }

        protected abstract string GetWorkingUserName(TUser workingUser);

        private FileUploadResult Upload(IFormFile postedFile, string saveTo)
        {

            string savedFileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);
            try
            {
                string filePath = Env.ContentRootPath + "\\" + saveTo;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                using (var stream = new FileStream(filePath + "\\" + savedFileName, FileMode.Create))
                {

                    postedFile.CopyTo(stream);
                }
                return new FileUploadResult
                {
                    OriginalFileName = Path.GetFileName(postedFile.FileName),
                    SavedFileName = savedFileName
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResult { ErrorMessage = ex.Message };
            }
        }
    }
}