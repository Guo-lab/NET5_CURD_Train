using ESC5.AppBase;
using ProjectBase.BusinessDelegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.WebCommon
{
    public class BaseWebApplicationStorageHelper : IBaseApplicationStorageHelper
    {
        public IApplicationStorage AppStorage { get; set; }

        public string UploadedFileFolder
        {
            get
            {
                return AppStorage.GetRealPath(AppStorage.GetAppSetting("DefaultFilePath")) + "\\";
            }
        }
    }
}
