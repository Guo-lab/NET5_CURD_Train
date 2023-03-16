using System.Collections.Generic;

namespace ImportLibrary
{
    public interface IImportBD
    {
        FeedbackResult<TImportItem> TypedImport<TImportItem>(string importName, string parserName, string fileName, params object[] parameters) where TImportItem : BaseImportItem;
        FeedbackResult<TImportItem> TypedImport<TImportItem>(string importName, string parserName, string fileName, IDictionary<string, object> parameterMap) where TImportItem : BaseImportItem;

        object Import(string importName, string parserName, string fileName, IDictionary<string, object> parameterMap);
        object Import(string importName, string parserName, string fileName, params object[] parameters);

    }

}
