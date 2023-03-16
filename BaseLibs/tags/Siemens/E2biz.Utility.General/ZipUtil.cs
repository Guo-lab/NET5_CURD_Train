using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESC5.Utility
{
    public class ZipUtil
    {
        public static string ZipFile(string[] filesToZip, string temporaryPath, string zipPassword = "", int blockSize = 2048, int zipLevel = 9)
        {
            try
            {
                var newFileName = DateTime.Now.ToString("yyyyMMdd") + ".zip";
                var zipedFilePath = temporaryPath + "\\" + newFileName;


                //创建压缩文件
                ZipOutputStream zipStream = new ZipOutputStream(System.IO.File.Create(zipedFilePath));
                zipStream.SetLevel(zipLevel);
                zipStream.Password = zipPassword;

                //写入所有文件到压缩文件
                for (int i = 0; i < filesToZip.Length; i++)
                {
                    string strFilePath = filesToZip[i];
                    FileStream fs = null;
                    try
                    {
                        //被压缩的文件名
                        string strFileName = strFilePath.Substring(strFilePath.LastIndexOf("\\") + 1);

                        ZipEntry entry = new ZipEntry(strFileName);
                        entry.DateTime = DateTime.Now;
                        zipStream.PutNextEntry(entry);

                        //读取文件
                        fs = System.IO.File.OpenRead(strFilePath);

                        //缓冲区大小
                        byte[] buffer = new byte[blockSize];
                        int sizeRead = 0;
                        do
                        {
                            sizeRead = fs.Read(buffer, 0, buffer.Length);
                            zipStream.Write(buffer, 0, sizeRead);
                        }
                        while (sizeRead > 0);
                    }
                    catch (Exception ex)
                    {
                        //continue;
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                            fs.Dispose();
                        }
                    }
                }

                zipStream.Finish();
                zipStream.Close();

                //返回压缩后的压缩文件相对路径
                return newFileName;

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


    }
}

