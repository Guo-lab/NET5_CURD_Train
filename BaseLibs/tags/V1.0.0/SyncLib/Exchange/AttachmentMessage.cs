using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Utils;
using System.IO;
using SyncLib.Exception;
using Newtonsoft.Json;
using ProjectBase.BusinessDelegate;
using ProjectBase.Application;

namespace SyncLib.Exchange
{
    public class AttachmentMessage:Message
    {
        private IApplicationStorage _appStorage;
        private IApplicationStorage ApplicationStorage
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

        private IUtil _util;
        private IUtil Util
        {
            get
            {
                if (_util == null)
                {
                    _util = CastleContainer.WindsorContainer.Resolve<IUtil>();
                }
                return _util;
            }
        }
        public string SavedFileName { get; set; }
        public string TargetFolder { get; set; }
        public override void SaveTo(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            File.WriteAllText(folder + "\\Attachment-" + Path.GetFileNameWithoutExtension (this.SavedFileName) + ".json", JsonConvert.SerializeObject(this));
        }

        public void SavetoTarget()
        {
            try
            {
                string directory = ApplicationStorage.GetRealPath(this.TargetFolder);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                int index = this.SavedFileName.IndexOf("\\");
                if (index != -1)
                {
                    string subFolder = this.SavedFileName.Substring(0,index);
                    if (!Directory.Exists(directory + "\\" + subFolder))
                    {
                        Directory.CreateDirectory(directory + "\\" + subFolder);
                    }
                }
                string fileName = directory + "\\" + this.SavedFileName;

                byte[] bytes = Convert.FromBase64String(this.MessageInfo);

                Stream stream = File.Create(fileName);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            catch (System.Exception ex)
            {
                Util.AddLog("Attachment.SavetoTarget", ex);
                throw new SaveFileException(this.SavedFileName);
            }
        }
    }
}
