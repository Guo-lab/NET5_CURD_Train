using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace UtilService.PDF
{
    public class ImagePDFConverter : IPDFConverter

    {
        public byte[] Convert(string sourceFile)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 0, 0, 0, 0);
                PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();

                //var imgStream = GetImageStream(sourceFile);
                var image = Image.GetInstance(sourceFile);
                image.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                document.Add(image);

                document.Close();
                return ms.ToArray();
            }
        }

    }
}
