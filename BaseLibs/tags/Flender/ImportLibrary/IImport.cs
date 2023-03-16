using System.Collections.Generic;

namespace ImportLibrary
{
    public interface IImport
    {
        IFileParser FileParser { get; set; }
        string Import();
        object Import2();
        FeedbackResult<T> Import<T>() where T:BaseImportItem;

        void SetImportParameters(params object[] importParameters);

        void SetImportParameters(IDictionary<string,object> importParameters);
    }
}
