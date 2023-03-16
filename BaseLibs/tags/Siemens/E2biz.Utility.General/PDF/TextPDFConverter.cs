using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace UtilService.PDF
{
    public class TextPDFConverter : IPDFConverter

    {
        public byte[] Convert(string sourceFile)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                StreamReader reader = new StreamReader(sourceFile);
                string fileContent = reader.ReadToEnd();
                reader.Close();

                Document document = new Document();
                PdfWriter.GetInstance(document, ms);
                document.Open();
                document.Add(new Paragraph(fileContent));
                document.Close();
                return ms.ToArray();
            }
        }
    }
}
