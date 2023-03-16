using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Aspose.Cells;

namespace UtilService.PDF
{
    public class ExcelPDFConverter : IPDFConverter
    {
        public byte[] Convert(string sourceFile)
        {
            Workbook workBook = new Workbook(sourceFile);
            using (var stream = new MemoryStream())
            {
                workBook.Save(stream, SaveFormat.Pdf);
                return stream.ToArray();
            }
        }
    }
}
