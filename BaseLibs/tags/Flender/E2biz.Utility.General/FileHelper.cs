using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace E2biz.Utility.General
{
    public class FileHelper
    {

            // 文件列表
            private static List<FileInfo> _FileList = new List<FileInfo>();

            #region   公有方法

            /// <summary>
            /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
            /// </summary>
            /// <param name="path">文件夹路径</param>
            /// <param name="extName">扩展名可以多个 例如[.mp4] [.mp3] [.wma] 等</param>
            /// <returns>List<FileInfo></returns>
            public static List<FileInfo> GetallFile(string path, string extName)
            {
                //检查目录是否存在
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (Directory.Exists(path))
                    {
                        GetallfilesOfDir(path, extName);
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                return _FileList;
            }



            #endregion



            #region   私有方法
            /// <summary>
            /// 递归获取指定类型文件,包含子文件夹
            /// </summary>
            /// <param name="path">指定文件夹的路径</param>
            /// <param name="extName">文件拓展名</param>
            private static void GetallfilesOfDir(string path, string extName)
            {
                try
                {
                    string[] dir = Directory.GetDirectories(path); //文件夹列表   
                    DirectoryInfo fdir = new DirectoryInfo(path);
                    FileInfo[] file = fdir.GetFiles();

                    //FileInfo[] file = Directory.GetFiles(path); //文件列表  

                    if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空                   
                    {
                        foreach (FileInfo f in file) //显示当前目录所有文件   
                        {
                            if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
                            {
                                _FileList.Add(f);
                            }
                        }
                        foreach (string d in dir)
                        {
                            GetallfilesOfDir(d, extName);//递归   
                        }
                    }
                }
                catch (Exception ex)
                {
                    //注意这里的EverydayLog.Write()是我自定义的日志文件，可以根据需要保留或删除
                 
                }
            }

            #endregion

        }//Class_end
    }




