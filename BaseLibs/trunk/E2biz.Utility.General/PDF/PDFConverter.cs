using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace UtilService.PDF
{
    public class PDFConverter : IPDFConverter
    {
        public byte[] Convert(string sourceFile)
        {

            return File.ReadAllBytes(sourceFile);
        }
    }
}
