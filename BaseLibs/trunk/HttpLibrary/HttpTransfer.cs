using System.Text;
using System.IO;
using System.Net;

namespace HttpLibrary
{
    public class HttpTransfer
    {
        public static string Open(string psURL, object o)
        {
            return Open(psURL, Obj2QS.ConvertToQS(o));
        }
        public static string Open(string psURL, string psContent)
        {
            return Open(psURL + "?_ForViewModelOnly=true&KeepDateFormat=1", psContent, "POST", System.Text.Encoding.UTF8);
        }

        public static string Open(string psURL, string psContent, string psMethod, Encoding encoding)
        {
            byte[] bs = encoding.GetBytes(psContent);

            HttpWebRequest oRequest = (HttpWebRequest)HttpWebRequest.Create(psURL);
            oRequest.Method = psMethod;
            oRequest.ContentType = "application/x-www-form-urlencoded";
            oRequest.ContentLength = bs.Length;
            
            using (Stream oRequestStream = oRequest.GetRequestStream())
            {
                oRequestStream.Write(bs, 0, bs.Length);
                oRequestStream.Close();
            }

            return encoding.GetString(Open(oRequest));
        }

        private static byte[] Open(HttpWebRequest poRequest)
        {
            using (HttpWebResponse oHttpWebResponse = (HttpWebResponse)poRequest.GetResponse())
            {
                Stream oResponseStream = oHttpWebResponse.GetResponseStream();
                byte[] bytes = new byte[1025];
                int nRead = 0;
                MemoryStream oMemoryStream = new MemoryStream();
                while (true)
                {
                    nRead = oResponseStream.Read(bytes, 0, bytes.Length);
                    if (nRead == 0)
                    {
                        break;
                    }
                    oMemoryStream.Write(bytes, 0, nRead);
                }
                // 设置当前流的位置为流的开始 
                oMemoryStream.Seek(0, SeekOrigin.Begin);
                oHttpWebResponse.Close();
                return oMemoryStream.ToArray();
            }


        }

    }
}
