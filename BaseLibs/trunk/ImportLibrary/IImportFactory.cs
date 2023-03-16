
using System;

namespace ImportLibrary
{
    [Obsolete]
    public interface IImportFactory
    {
        IImport CreateImport(string fullName);        
    }
}
