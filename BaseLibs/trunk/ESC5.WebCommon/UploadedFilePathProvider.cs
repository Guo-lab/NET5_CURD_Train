using ESC5.AppBase;
using ProjectBase.Web.Mvc.ValueInFile;

namespace ESC5.WebCommon
{
    public class UploadedFilePathProvider : IValueInFilePathProvider
    {
        public IBaseApplicationStorageHelper ApplicationHelper { get; set; }
        public string GetFilePath(string fileName)
        {
            return ApplicationHelper.UploadedFileFolder + fileName;
        }
    }
}
