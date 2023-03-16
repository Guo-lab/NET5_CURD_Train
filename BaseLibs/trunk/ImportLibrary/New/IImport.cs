using System.Collections.Generic;
using System;

namespace ImportLibrary.New
{
    public interface IImport<T> where T : BaseImportItem
    {
        FeedbackResult<T> Import(IEnumerable<T> boundImportItems);
        FeedbackResult<T> Import(IEnumerable<T> boundImportItems, params object[] importParameters);
        FeedbackResult<T> Import(IEnumerable<T> boundImportItems, IDictionary<string, object> importParameters);
    }
}
