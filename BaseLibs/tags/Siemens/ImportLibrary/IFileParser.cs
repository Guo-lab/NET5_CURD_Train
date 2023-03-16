using System;
using System.Collections.Generic;

namespace ImportLibrary
{
    public interface IFileParser
    {
        IEnumerable<BaseImportItem> Parse();

        string FileName
        {
            get
            {
                throw new Exception("实现类应实现此属性");
            }
            set
            {
                throw new Exception("实现类应实现此属性");
            }
        }

    }
}
