using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectBase.Web.Mvc.ValueInFile
{
        public interface IValueInFileParser
    {
            IList<Dictionary<string, string>> ParseAsNV(FileInfo file);
        }
}