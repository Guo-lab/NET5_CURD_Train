using System;
using System.Collections.Generic;
using ProjectBase.Utils;
using ImportLibrary.Validate;
using System.Linq;
namespace ImportLibrary.New
{
    public abstract class BaseImporter<T, ResultT> : IImport<T> where T : BaseImportItem
                                                             where ResultT : BaseImportResult<T>,new()
    {

        public IUtil Util { get; set; }
        public IImportValidateController<T> ValidateController { get; set; }

        public ResultT Result { get; set; }

        protected object[] ImportParameters { get; set; }
        protected IDictionary<string, object> ImportParametersMap { get; set; }

        public BaseImporter()
        {
            Result = new ResultT();
        }

        public BaseImporter(ResultT poImportResult)
        {
            Result = poImportResult;
        }

        protected virtual void Validate()
        {
            ValidateController.ImportItems = Result.ImportItems;
            ValidateController.StartValidate();
        }
        protected abstract void DoImport();

        //验证之前对导入的数据做预处理，例如根据Code找到对应的Domain对象，这样在做验证和实际导入的时候，就不必多次查数据库了
        protected virtual void PreProcess()
        {
        }

        private FeedbackResult<T> InternalImport(IEnumerable<T> boundImportItems =null) 
        {
            Result.ImportItems = boundImportItems;
            
            if (Result.ImportItems == null)
            {
                Result.ImportResult = ImportResult.InvalidFile;
            }
            else
            {
                PreProcess();
                Validate();
                DoImport();
                Result.ImportResult = ImportResult.Succeed;
            }

            return Result.GetFeedBackResult();
        }

        public FeedbackResult<T> Import(IEnumerable<T> boundImportItems) 
        {
            return InternalImport(boundImportItems);
        }
        public FeedbackResult<T> Import(IEnumerable<T> boundImportItems, params object[] importParameters)
        {
            ImportParameters = importParameters;
            return InternalImport(boundImportItems);
        }

        public FeedbackResult<T> Import(IEnumerable<T> boundImportItems, IDictionary<string, object> importParameters)
        {
            ImportParametersMap = importParameters;
            return InternalImport(boundImportItems);
        }
    }

    public abstract class BaseImporter<T> : BaseImporter<T, BaseImportResult<T>>,IImport<T> where T : BaseImportItem
    {

    }
}
