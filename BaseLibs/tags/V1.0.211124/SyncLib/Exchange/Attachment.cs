using System;
using System.IO;
using SyncLib.Exception;
using ProjectBase.Utils;
using ProjectBase.BusinessDelegate;
using ProjectBase.Application;

namespace SyncLib.Exchange
{
    public class Attachment
    {
        private IApplicationStorage _appStorage;
        private IApplicationStorage ApplicationStorage { get
            {
                if (_appStorage == null)
                {
                    _appStorage = CastleContainer.WindsorContainer.Resolve<IApplicationStorage>();
                }
                return _appStorage;
            }
        }

        private IUtil _util;
        private IUtil Util { get
            {
                if (_util == null)
                {
                    _util = CastleContainer.WindsorContainer.Resolve<IUtil>();
                }
                return _util;
            }
        }
        protected string SourceFolder { get; set; }
        public string TargetFolder { get; protected set; }
        public Attachment(string sourceFolder, string targetFolder)
        {
            this.SourceFolder = sourceFolder;
            this.TargetFolder = targetFolder;
        }
        public string OriginalFileName { get; set; }
        public string SavedFileName { get; set; }

        public void SetSourceFolder(string sourceFolder)
        {
            this.SourceFolder = sourceFolder;
        }
        public string GetAttachmentContent()
        {
            try
            {
                string fileName;
                if (Path.IsPathRooted(this.SourceFolder))
                {
                    fileName = this.SourceFolder + "\\" + this.SavedFileName;
                }
                else {
                    fileName = ApplicationStorage.GetRealPath(this.SourceFolder) + "\\" + this.SavedFileName;
                }
                if (!File.Exists(fileName))
                {
                    throw new FileMissingException(fileName);
                }

                FileInfo oFileInfo = new FileInfo(fileName);
                long lFileSize = oFileInfo.Length;
                byte[] arrByte = new byte[lFileSize];
                Stream oStream = File.OpenRead(fileName);
                int nRead = 0;
                int nOffset = 0;
                int nAvailabelBytes;
                if (lFileSize > int.MaxValue)
                    nAvailabelBytes = int.MaxValue;
                else
                    nAvailabelBytes = (int)lFileSize;
                while (true)
                {
                    nRead = oStream.Read(arrByte, nOffset, nAvailabelBytes);
                    if (nRead == 0)
                        break;
                    nOffset += nRead;
                    if (lFileSize - nOffset > int.MaxValue)
                        nAvailabelBytes = int.MaxValue;
                    else
                        nAvailabelBytes = (int)lFileSize - nOffset;
                }
                oStream.Close();
                return Convert.ToBase64String(arrByte);
            }
            catch(FileMissingException)
            {
                throw ;
            }
            catch (System.Exception ex)
            {
                Util.AddLog("Attachment.GetAttachmentContent", ex);
                throw new ReadFileException(this.SavedFileName);
            }
        }
    }
}
