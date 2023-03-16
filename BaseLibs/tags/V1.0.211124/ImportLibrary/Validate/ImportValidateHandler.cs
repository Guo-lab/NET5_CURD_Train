using System.Collections.Generic;
using System.Linq;
using System;

namespace ImportLibrary.Validate
{
    public abstract class ImportValidateHandler<T> : IImportValidateHandler<T> where T : BaseImportItem
    {
        private IImportValidateHandler<T> NextHandler { set; get; }

        public IImportValidateHandler<T> SetNextHandler(IImportValidateHandler<T> handler)
        {
            this.NextHandler = handler;
            return handler;
        }



        // 每个验证者都必须对请求做出处理
        public void HandleValidate(IEnumerable<T> item)
        {
            this.Validate(item);
            // 判断是否有下一个验证者
            if (this.NextHandler != null)
            {
                this.NextHandler.HandleValidate(item);
            }
        }

        // 每个验证者都必须实现验证任务
        protected abstract void Validate(IEnumerable<T> item);
        protected void AppendErrorMessage(IEnumerable<T> items, string message)
        {
            AppendErrorMessage(items, message, ImportLineResult.Failed);
        }
        protected void AppendWarningMessage(IEnumerable<T> items, string message)
        {
            AppendErrorMessage(items, message, ImportLineResult.Warning);
        }
        protected void AppendErrorMessage(IEnumerable<T> items, string message, ImportLineResult result)
        {
            foreach (BaseImportItem item in items)
            {
                if (string.IsNullOrEmpty(item.ErrorMessage))
                {
                    item.ErrorMessage = message;
                }
                else
                {
                    item.ErrorMessage += "," + message;
                }
                if (!item.Result.HasValue || result > item.Result.Value)
                {
                    item.Result = result;
                }
            }
        }
    }
}
