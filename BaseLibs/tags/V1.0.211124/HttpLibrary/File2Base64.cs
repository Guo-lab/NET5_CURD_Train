using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

namespace HttpLibrary
{
    public class File2Base64
    {
        public static string ConvertToBase64(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            FileInfo oFileInfo = new FileInfo(fileName);
            long lFileSize = oFileInfo.Length;
            byte[] arrByte = new byte[lFileSize + 1];
            Stream oStream = File.OpenRead(fileName);
            int nRead = 0;
            int nOffset = 0;
            int nAvailabelBytes;
            if (lFileSize > int.MaxValue)
                nAvailabelBytes = int.MaxValue;
            else
                nAvailabelBytes = (int)lFileSize;
            while (true)
            {
                nRead = oStream.Read(arrByte, nOffset, nAvailabelBytes);
                if (nRead == 0)
                    break;
                nOffset += nRead;
                if (lFileSize - nOffset > int.MaxValue)
                    nAvailabelBytes = int.MaxValue;
                else
                    nAvailabelBytes = (int)lFileSize - nOffset;
            }
            oStream.Close();
            return  Convert.ToBase64String(arrByte);

            
        }
    }
}
