using Aspose.Words;
using System.IO;

namespace UtilService.PDF
{
    public class WordPDFConverter : IPDFConverter
    {
        public byte[] Convert(string sourceFile)
        {
            Document doc = new Document(sourceFile);
            using (MemoryStream stream = new MemoryStream())
            {
                doc.Save(stream, SaveFormat.Pdf);
                return stream.ToArray();
            }
        }
    }
}
