using ProjectBase.BusinessDelegate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ESC5.ValueObject;

namespace ESC5.WebCommon
{
    public class AttachmentHandler:IAttachmentHandler
    {
        public IApplicationStorage AppStorage { get; set; }
        public void MoveToFormalFolder(AttachmentVM attachment)
        {
            if (attachment.Temporary)
            {
                string tempFile = AppStorage.GetRealPath(AppStorage.GetAppSetting("DefaultFilePath")) + "\\" + attachment.SavedFileName;
                if (File.Exists(tempFile))
                {
                    string targetFile = DateTime.Today.ToString("yyyyMMdd") + "\\" + attachment.SavedFileName;
                    string targetFolder = AppStorage.GetRealPath(attachment.SavedFolder) + "\\" + DateTime.Today.ToString("yyyyMMdd");
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                    File.Move(tempFile, AppStorage.GetRealPath(attachment.SavedFolder) + "\\" + targetFile);
                }
            }
        }
    }
}
