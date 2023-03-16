using System.Collections.Generic;

namespace ImportLibrary.Validate
{
    public interface IImportValidateHandler<T> where T : BaseImportItem
    {
        void HandleValidate(IEnumerable<T> item);
        IImportValidateHandler<T> SetNextHandler(IImportValidateHandler<T> handler);
    }
}