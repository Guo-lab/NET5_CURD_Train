
using System;

namespace ImportLibrary
{
    [Obsolete]
    public interface IFileParserFactory
    {
        IFileParser CreateParser(string fullName);        
    }
}
