using System.Collections.Generic;
using System;
namespace ImportLibrary
{
	[Obsolete]
    public interface IImportBD
    {
        [Obsolete]FeedbackResult<TImportItem> TypedImport<TImportItem>(string importName, string parserName, string fileName, params object[] parameters) where TImportItem : BaseImportItem;
        [Obsolete]FeedbackResult<TImportItem> TypedImport<TImportItem>(string importName, string parserName, string fileName, IDictionary<string, object> parameterMap) where TImportItem : BaseImportItem;

        [Obsolete]object Import(string importName, string parserName, string fileName, IDictionary<string, object> parameterMap);
        [Obsolete]object Import(string importName, string parserName, string fileName, params object[] parameters);

    }

}
