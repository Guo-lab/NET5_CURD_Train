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

        public void MoveToFormalFolder(AttachmentVM attachment, bool withDateFolder) {
            if (withDateFolder) {
                MoveToFormalFolder(attachment);
            }
            else
            {
                MoveToFormalFolder(attachment, attachment.SavedFileName);
            }
        }

        public void MoveToFormalFolder(AttachmentVM attachment, string saveAs = "")
        {
            if (attachment.Temporary)
            {
                string tempFile = AppStorage.GetRealPath(AppStorage.GetAppSetting("DefaultFilePath")) + "\\" + attachment.SavedFileName;
                if (File.Exists(tempFile))
                {
                    string targetFile = saveAs == "" ? DateTime.Today.ToString("yyyyMMdd") + "\\" + attachment.SavedFileName : saveAs;
                    string targetFolder = saveAs == "" ? AppStorage.GetRealPath(attachment.SavedFolder) + "\\" + DateTime.Today.ToString("yyyyMMdd") : AppStorage.GetRealPath(attachment.SavedFolder);
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                    File.Move(tempFile, AppStorage.GetRealPath(attachment.SavedFolder) + "\\" + targetFile, true);
                }
            }
        }
    }
}
