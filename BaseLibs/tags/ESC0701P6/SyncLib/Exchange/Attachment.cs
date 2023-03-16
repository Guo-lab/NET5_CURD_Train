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

                FileInfo fileInfo = new FileInfo(fileName);
                long fileSize = fileInfo.Length;
                byte[] bytes = new byte[fileSize];
                Stream stream = File.OpenRead(fileName);
                int readCount = 0;
                int offset = 0;
                int availabelBytes;
                if (fileSize > int.MaxValue)
                    availabelBytes = int.MaxValue;
                else
                    availabelBytes = (int)fileSize;
                while (true)
                {
                    readCount = stream.Read(bytes, offset, availabelBytes);
                    if (readCount == 0)
                        break;
                    offset += readCount;
                    if (fileSize - offset > int.MaxValue)
                        availabelBytes = int.MaxValue;
                    else
                        availabelBytes = (int)fileSize - offset;
                }
                stream.Close();
                return Convert.ToBase64String(bytes);
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
