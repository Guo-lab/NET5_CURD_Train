using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ProjectBase.Utils
{
    public interface IUtil
    {
        string FuncTree { get; }
        Dictionary<string, int> FuncMap { get; }
        void AddLog(string psSource);
        void AddLog(string psSource, Exception ex);
        string DesDecrypt(string pToDecrypt, string desKey);
        string DesEncrypt(string pToEncrypt, string desKey);
        Stream GetBarcodeImage(string psBarCodeText, int pnWidth, int pnHeight);
        Stream GetBarcodeImage(string psBarCodeText, int pnWidth, int pnHeight, bool pbShowFooter);
        string GetClientIP();
        string GetWebRoot();
        bool IsDevelopment();
        bool IsIP(string ip);
        bool IsNullableType(Type theType);
        bool IsValidEmail(string email);
        string NgControllerJs();
        bool IsAjaxRequest(HttpRequest request);
        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        //string MD5Encrypt(string str);
        String CalcCheckCode(params Object[] data);
    }
}