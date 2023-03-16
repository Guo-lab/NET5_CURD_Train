using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
namespace UtilService.PDF
{
    public class PDFMerger
    {
        public static void MergePDF(IList<byte[]> byteArray, string targetFile)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document doc = new Document();

                PdfCopy pdf = new PdfCopy(doc, stream);
                pdf.CloseStream = false;
                doc.Open();

                PdfReader reader = null;
                PdfImportedPage page = null;

                foreach (byte[] bytes in byteArray)
                {
                    reader = new PdfReader(bytes);
                    for (int i = 0; i < reader.NumberOfPages; i++)
                    {
                        page = pdf.GetImportedPage(reader, i + 1);
                        pdf.AddPage(page);
                    }

                    pdf.FreeReader(reader);
                    reader.Close();
                }
                doc.Close();

                using (FileStream streamX = new FileStream(targetFile, FileMode.Create))
                {
                    stream.WriteTo(streamX);
                }
            }
        }
    }
}
