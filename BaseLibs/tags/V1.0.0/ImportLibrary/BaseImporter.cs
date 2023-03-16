using System;
using System.Collections.Generic;
using ProjectBase.Utils;
using ImportLibrary.Validate;
using System.Linq;
namespace ImportLibrary
{
    public enum ImportResult {
        InvalidFile = 0,
        UnexpectedResult = 1,
        Succeed = 2
    }

    public abstract class BaseImporter<T, ResultT> : IImport where T : BaseImportItem
                                                             where ResultT:BaseImportResult<T>,new()
    {

        public IUtil Util { get; set; }
        public IFileParser FileParser { get; set; }
        public IImportValidateController<T> ValidateController { get; set; }
        public BaseImporter()
        {
            this.Result = new ResultT();
        }

        public BaseImporter(IFileParser poFileParser)
        {
            this.FileParser = poFileParser;
            this.Result = new ResultT();
        }

        public BaseImporter(IFileParser poFileParser, ResultT poImportResult)
        {
            this.FileParser = poFileParser;
            this.Result = poImportResult;
        }

        protected object[] ImportParameters { get; set; }
        public void SetImportParameters(params object[] importParameters)
        {
            this.ImportParameters = importParameters;
        }

        public ResultT Result
        {
            get; set;
        }
        protected virtual void Validate()
        {
            ValidateController.ImportItems = this.Result.ImportItems;
            ValidateController.StartValidate();
        }
        protected abstract void DoImport();

        //验证之前对导入的数据做预处理，例如根据Code找到对应的Domain对象，这样在做验证和实际导入的时候，就不必多次查数据库了
        protected virtual void PreProcess()
        {
        }
        public string Import()
        {
            try
            {
                this.Result.ImportItems = (IEnumerable<T>)this.FileParser.Parse();
                if (this.Result.ImportItems == null)
                {
                    this.Result.ImportResult = ImportResult.InvalidFile;
                }
                else
                {
                    this.Result.ImportItems.ToList().ForEach(x => x.Result = ImportLineResult.Succeed);
                    PreProcess();
                    Validate();
                    DoImport();
                    this.Result.ImportResult = ImportResult.Succeed;
                }
            }
            catch (Exception ex)
            {
                if (ex is ProjectBase.BusinessDelegate.BizException)
                {
                    throw ;
                    //this.Result.ImportResult = ImportResult.InvalidFile;
                    //this.Result.ErrorMessage = ex.Message;
                }
                else {
                    Util.AddLog("BaseImporter.Import", ex);
                    this.Result.ImportResult = ImportResult.UnexpectedResult;
                }
            }
            return this.Result.GetResultJson();
        }
    }

    public abstract class BaseImporter<T> : BaseImporter<T,BaseImportResult<T>>  where T : BaseImportItem
    {
        
    }
}
