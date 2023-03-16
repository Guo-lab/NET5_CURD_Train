using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ImportLibrary
{
    public class FeedbackResult<T> where T :BaseImportItem
    {
        public int SucceedCount { get; set; }
        public int WarningCount { get; set; }
        public int FailedCount { get; set; }
        public IEnumerable<T> Items { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ErrorOnlyImportResult<T>:BaseImportResult<T> where T : BaseImportItem
    {
        protected override string GetSucceedMessage()
        {
            return JsonConvert.SerializeObject(new FeedbackResult<T>
            {
                FailedCount = ImportItems.Count(x => x.Result == ImportLineResult.Failed),
                WarningCount = ImportItems.Count(x => x.Result == ImportLineResult.Warning),
                SucceedCount = ImportItems.Count(x => x.Result == ImportLineResult.Succeed),
                Items = ImportItems.Where(x => x.Result == ImportLineResult.Failed)
            });
        }
    }
    public class BaseImportResult<T> where T : BaseImportItem
    {
        public IEnumerable<T> ImportItems { get; set; }
        public ImportResult ImportResult { get; set; }
        public string ErrorMessage { get; set; }
        public string GetResultJson()
        {
            if (this.ImportResult == ImportResult.InvalidFile)
            {
                if (string.IsNullOrEmpty(this.ErrorMessage))
                {
                    return JsonConvert.SerializeObject(new { ErrorMessage = "请检查文件格式是否正确" });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { ErrorMessage = this.ErrorMessage });
                }
            }
            else if (this.ImportResult == ImportResult.UnexpectedResult)
            {
                return JsonConvert.SerializeObject(new { ErrorMessage = "意外错误，请联系系统管理员" });
            }
            else {
                return GetSucceedMessage();
            }
        }

        protected virtual string GetSucceedMessage()
        {
            return JsonConvert.SerializeObject(new FeedbackResult<T>
            {
                FailedCount= ImportItems.Count(x => x.Result == ImportLineResult.Failed),
                WarningCount = ImportItems.Count(x => x.Result == ImportLineResult.Warning),
                SucceedCount = ImportItems.Count(x => x.Result == ImportLineResult.Succeed),
                Items = ImportItems
            });
        }
    }
}
