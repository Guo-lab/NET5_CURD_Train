using Newtonsoft.Json;
using ProjectBase.Application;
using ProjectBase.BusinessDelegate;
using System.IO;
using System;

namespace ESC5.ValueObject
{
    public class AttachmentVM
    {
        public AttachmentVM() { }
        public AttachmentVM(string savedFolder)
        {
            this.SavedFolder = savedFolder;
        }
        public string OriginalFileName { get; set; }
        public string SavedFileName { get; set; }

        //指示当前附件是否在临时文件夹。保存数据的时候会把附件移动到正式的存储文件夹下
        //上传附件时会根据存储目录设置为true或者false，从数据库中读取的总是为false
        private bool? _temporary;
        public bool Temporary
        {
            get
            {
                if (_temporary.HasValue)
                {
                    return _temporary.Value;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                _temporary = value;
            }
        }
        public string url
        {
            get
            {
                return "../Download/DownloadAttachment?psSaveTo=" + this.SavedFolder + "&psOriginalFileName=" + System.Web.HttpUtility.UrlEncode(this.OriginalFileName) + "&psSavedFileName=" + this.SavedFileName;
            }
        }

        [JsonIgnore]
        public string SavedFolder { get; set; }

        private static IApplicationStorage _appStorage;
        [JsonIgnore]
        public static IApplicationStorage AppStorage
        {
            get
            {
                if (_appStorage == null)
                {
                    _appStorage = CastleContainer.WindsorContainer.Resolve<IApplicationStorage>();
                }
                return _appStorage;
            }
        }

        public long size
        {
            get
            {
                try
                {
                    return new FileInfo(AppStorage.GetRealPath(this.SavedFolder) + "\\" + SavedFileName).Length;
                }
                catch
                {
                    return 0;
                }
            }
        }

        [JsonIgnore]
        public string SavedFileNameWithFolder
        {
            get
            {
                if (this.SavedFileName.Contains("\\"))
                {
                    return this.SavedFileName;
                }
                else
                {
                    return DateTime.Today.ToString("yyyyMMdd") + "\\" + this.SavedFileName;
                }
                        
            }
        }
        
    }
}
