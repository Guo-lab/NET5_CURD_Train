using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using ProjectBase.BusinessDelegate;
using ProjectBase.Data;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ZXing;

namespace ProjectBase.Utils
{
    public class Util : IUtil
    {
        public IHttpContextAccessor Hca { get; set; }
        public IApplicationStorage AppState { get; set; }
        public IHostEnvironment Env { get; set; }
        public IUtilQuery UtilQuery { get; set; }

        public string _logRoot;
        private string _key_For_DictMap = "Key_For_DictMap";
        private string _key_For_FuncMap = "Key_For_FuncMap";
        private string _key_For_FuncTree = "Key_For_FuncTree";

        public bool IsDevelopment() => Env.IsDevelopment();
        public virtual string LogRoot
        {
            get
            {

                if (_logRoot == null)
                {
                    string logroot = AppState.GetAppSetting("LogRoot");
                    if (!string.IsNullOrEmpty(logroot))
                    {
                        _logRoot = AppState.GetRealPath(logroot);
                    }
                }
                return _logRoot;
            }
            set
            {
                _logRoot = value;
            }
        }

        //db dictionary items map
        public Dictionary<string, string> DictMap
        {
            get
            {
                if (AppState == null) throw new Exception("Util has not been initiated yet!");
                if (AppState.Get(_key_For_DictMap) == null)
                {
                    //load dict items from db and keep the map in memory
                    var list = UtilQuery.StatelessGetBySql("select isnull(a.ConstName+'.'+b.ConstName,''),b.ItemName from GN_DictItem b inner join GN_Dict a on b.DictID=a.ID");
                    IDictionary<string, string> map = list.ToDictionary(o => o[0] as string, o => o[1] as string);
                    map["ROV"] = "R.O.V";
                    map["Undefined"] = "";
                    AppState.Set(_key_For_DictMap, map);
                }

                return AppState.Get(_key_For_DictMap) as Dictionary<string, string>;
            }
        }
        //db func map
        public Dictionary<string, int> FuncMap
        {
            get
            {
                if (AppState == null) throw new Exception("Util has not been initiated yet!");
                if (AppState.Get(_key_For_FuncMap) == null)
                {
                    //load from db and keep the map in memory
                    var list = UtilQuery.StatelessGetBySql("select Code,Id from GN_Func");
                    AppState.Set(_key_For_FuncMap, list.ToDictionary(o => o[0] as string, o => (int)o[1]));
                }

                return AppState.Get(_key_For_FuncMap) as Dictionary<string, int>;
            }
        }
        //generate html for func checkboxtree once 
        public virtual string FuncTree
        {
            get
            {
                if (AppState == null) throw new Exception("Util has not been initiated yet!");
                if (AppState.Get(_key_For_FuncTree) == null)
                {
                    //load from db and keep the tree in memory
                    var list = UtilQuery.StatelessGetBySql("select Id,Level,Code,Name from GN_Func Order by Level");
                    String html0 = "<div class='row' pb-init-var='d' pb-init-data='{";
                    String html = "";
                    int activelevellength = 2;
                    String ids = "";
                    String levels = "";
                    for (var i = 0; i < list.Count; i++)
                    {
                        ids = ids + "," + list[i][0];
                        var currentlevel = (string)list[i][1];
                        levels = levels + ",\"" + currentlevel + "\"";
                        var nextlevel = "00";
                        if (i + 1 < list.Count)
                        {
                            nextlevel = (string)list[i + 1][1];
                        }
                        if (currentlevel.Length == 2)
                        {
                            if (i > 0 && ((String)list[i - 1][1]).Length != 2)
                            {
                                html = html + "</ul></div>";
                            }
                            html = html + "<div class='panel panel-default col-md-6'><ul style='list-style:none'>";
                        }
                        String omg = "d.isCollapsed[\"" + currentlevel + "\"]";
                        String lihtml = "";
                        if (nextlevel.Length > currentlevel.Length)
                        {
                            lihtml = "<li  style='list-style:none'><span class='glyphicon' ng-class='" + omg + "?\"glyphicon-plus\":\"glyphicon-minus\"' ng-click='" + omg + "=!" + omg + "'></span>";
                        }
                        else
                        {
                            lihtml = "<li style='list-style:none'>";
                        }
                        if (currentlevel.Length > 2 && currentlevel.Length > activelevellength)
                        {
                            String parentlevel = currentlevel.Substring(0, currentlevel.Length - 2);
                            html = html + "<ul style='list-style:none;padding-left:40px' uib-collapse='d.isCollapsed[\"" + parentlevel + "\"]'>";
                            html = html + lihtml + "<input type='checkbox' ng-change='d.chk_change(\"" + currentlevel + "\")' ng-model='d.selectedList[" + i + "]' ><label>" +
                                   list[i][3] + "</label>";
                            activelevellength = activelevellength + 2;
                        }
                        else if (currentlevel.Length == activelevellength || currentlevel.Length == 2)
                        {
                            html = html + lihtml + "<input type='checkbox' ng-change='d.chk_change(\"" + currentlevel + "\")' ng-model='d.selectedList[" + i + "]' ><label>" +
                                   list[i][3] + "</label>";
                        }
                        if (nextlevel.Length < activelevellength)
                        {
                            var times = (activelevellength - nextlevel.Length) / 2;//两位一级
                            if (nextlevel.Length == 2) times = times - 1;
                            for (var j = 0; j <= times; j++)
                                html = html + "</ul>";
                            activelevellength = activelevellength - (times + 1) * 2;
                        }
                    }
                    html = html + "</div>";
                    html = html0 + "\"funcList\":[" + ids.Substring(1) + "]," + "\"levelList\":[" + levels.Substring(1) + "]}'>\n" + html;
                    AppState.Set(_key_For_FuncTree, html);
                }

                return AppState.Get(_key_For_FuncTree) as string;
            }
        }

        private object _instance = new object();
        public virtual void AddLog(string psSource, Exception ex)
        {
            lock (_instance)
            {
                var sMessage = GetExceptionMsg(ex);

                var sFileName = LogRoot + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                System.IO.TextWriter oWriter;
                if (System.IO.File.Exists(sFileName))
                {
                    oWriter = System.IO.File.AppendText(sFileName);
                }
                else
                {
                    var oFileInfo = new System.IO.FileInfo(sFileName);
                    oWriter = oFileInfo.CreateText();
                }
                oWriter.WriteLine("Source:" + psSource);
                oWriter.WriteLine("Time:" + DateTime.Now);
                if (ex != null)
                {
                    oWriter.WriteLine("StackTrace:" + ex.StackTrace);
                    oWriter.WriteLine("Message:" + sMessage);
                }
                oWriter.WriteLine("----------------------------------");
                oWriter.Close();
            }
        }
        public virtual void AddLog(string psSource)
        {
            AddLog(psSource, null);
        }

        private string GetExceptionMsg(Exception ex)
        {
            if (ex == null) return null;
            var sMsg = ex.InnerException != null ? GetExceptionMsg(ex.InnerException) : ex.ToString();

            sMsg = sMsg + "\r\n" + ex.Message;
            return sMsg;
        }
        public bool IsNullableType(Type theType)
        {
            return theType.IsGenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        public string DesEncrypt(string pToEncrypt, string desKey)
        {
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(desKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(desKey);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            cs.Close();

            string str = Convert.ToBase64String(ms.ToArray());
            ms.Close();
            return str;

        }
        public string DesDecrypt(string pToDecrypt, string desKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

            var des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(desKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(desKey);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            cs.Close();
            var s = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return s;
        }

        public string NgControllerJs()
        {
            //if (!Env.IsDevelopment()) return "";
            var contentRoot = AppState.GetRealPath("");
            var fileNames = Directory.EnumerateFiles(contentRoot, "*Ctrl.js", SearchOption.AllDirectories).Where(x => !x.Contains("\\bin") && !x.Contains("\\obj"));
            var scripts = fileNames.OrderBy(x => Path.GetFileNameWithoutExtension(x)).Aggregate("", (aggregate, fileName) =>
                                  {
                                      var content = File.ReadAllText(fileName);
                                      return aggregate + content + "\r\n";
                                  });
            fileNames = Directory.EnumerateFiles(contentRoot + "/Shared/Directive/", "*.js", SearchOption.AllDirectories).Where(x => !x.Contains("\\bin") && !x.Contains("\\obj")); ;
            scripts = fileNames.Aggregate(scripts, (aggregate, fileName) =>
            {
                var content = File.ReadAllText(fileName);
                return aggregate + content + "\r\n";
            });

            return scripts;
        }
        public bool IsValidEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, "^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$");
        }

        public bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        }
        public string GetClientIP()
        {
            string result = String.Empty;

            result = Hca.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR");
            if (string.IsNullOrEmpty(result))
            {
                result = Hca.HttpContext.GetServerVariable("REMOTE_ADDR");
            }

            if (string.IsNullOrEmpty(result))
            {
                result = Hca.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            }

            if (string.IsNullOrEmpty(result) || !IsIP(result))
            {
                return "127.0.0.1";
            }

            return result;

        }
        public string GetWebRoot()
        {
            return new System.Text.StringBuilder()
                .Append(Hca.HttpContext.Request.Scheme)
                .Append("://")
                .Append(Hca.HttpContext.Request.Host)
                .Append(Hca.HttpContext.Request.PathBase)
                .ToString();
        }

        /// <summary>
        /// 生成CODE_128的一维条码
        /// </summary>
        /// <param name="psBarCodeText">条码内容</param>
        /// <param name="pnWidth">条码长度</param>
        /// <param name="pnHeight">条码高度</param>
        /// <returns></returns>
        public Stream GetBarcodeImage(string psBarCodeText, int pnWidth, int pnHeight, bool pbShowFooter)
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
        public Stream GetBarcodeImage(string psBarCodeText, int pnWidth, int pnHeight)
        {
            return GetBarcodeImage(psBarCodeText, pnWidth, pnHeight, false);
        }

        public bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers.ContainsKey("X-Requested-With")
                    && ((string)request.Headers["X-Requested-With"]).Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        }

        //public string MD5Encrypt(string str)
        //{
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] fromData = System.Text.Encoding.Default.GetBytes(str);
        //    byte[] targetData = md5.ComputeHash(fromData);
        //    string byte2String = null;
        //    for (int i = 0; i < targetData.Length; i++)
        //    {
        //        byte2String += targetData[i].ToString("x2");
        //    }
        //    return byte2String;
        //}
        public String CalcCheckCode(params Object[] data)
        {
            var all = "";
            foreach (Object item in data)
            {
                all += item?.ToString() ?? "";
            }
            return SHA256.GenerateHash(all,"");
        }
    }


}