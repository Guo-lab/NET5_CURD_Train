using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ZXing;

namespace E2biz.Utility.General
{
    public class ImageUtil
    {

        /// <summary>
        /// 生成CODE_128的一维条码
        /// </summary>
        /// <param name="psBarCodeText">条码内容</param>
        /// <param name="pnWidth">条码长度</param>
        /// <param name="pnHeight">条码高度</param>
        /// <returns></returns>
        public static Stream GetBarcodeImage(string psBarCodeText, int pnWidth, int pnHeight, bool pbShowFooter)
        {
            BarcodeWriterGeneric<Bitmap> oWriter = new BarcodeWriterGeneric<Bitmap>();
            oWriter.Format = BarcodeFormat.CODE_128;
            oWriter.Options = new ZXing.Common.EncodingOptions { Width = pnWidth, Height = pnHeight, PureBarcode = !pbShowFooter, Margin = 0 };
            //MultiFormatWriter mutiWriter = new MultiFormatWriter();
            //ZXing.Common.BitMatrix bm = mutiWriter.encode(psBarCodeText, BarcodeFormat.CODE_128, pnWidth, pnHeight);
            Bitmap bmp = oWriter.Write(psBarCodeText);
            System.IO.MemoryStream outStream = new System.IO.MemoryStream();
            bmp.Save(outStream, ImageFormat.Png);
            bmp.Dispose();
            return outStream;
        }
        public static Stream GetBarcodeImage(string psBarCodeText, int pnWidth, int pnHeight)
        {
            return GetBarcodeImage(psBarCodeText, pnWidth, pnHeight, false);
        }
        public static void SaveThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片 
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode =InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
             new Rectangle(x, y, ow, oh),
             GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
            }
            catch
            {
                throw;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }
}
