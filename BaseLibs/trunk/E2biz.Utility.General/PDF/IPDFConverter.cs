using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace UtilService.PDF
{
    public interface IPDFConverter
    {
        byte[] Convert(string sourceFile);
    }

    public class PDFConverterFactory
    {
        public static IPDFConverter CreateConverter(string fileName)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".xls":
                case ".xlsx":
                    return new ExcelPDFConverter();
                case ".doc":
                case ".docx":
                    return new WordPDFConverter();
                //case ".ppt":
                //case ".pptx":
                //    return new PowerpointPDFConverter();
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".tiff":
                case ".tif":
                case ".png":
                    return new ImagePDFConverter();
                case ".txt":
                    return new TextPDFConverter();
                case ".pdf":
                    return new PDFConverter();
                default:
                    return null;
            }
        }
    }
}
