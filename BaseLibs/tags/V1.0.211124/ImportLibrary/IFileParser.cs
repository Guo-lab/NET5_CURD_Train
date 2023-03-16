using System;
using System.Collections.Generic;

namespace ImportLibrary
{
    public interface IFileParser
    {
        IEnumerable<BaseImportItem> Parse();
    }
}
