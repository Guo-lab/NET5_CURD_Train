using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ProjectBase.Web.Mvc.Angular
{
    //服务器响应回到客户端后指示客户端应完成的动作
    public class RichClientJsonResult : JsonResult
    {
        public static string Key_KeepDateFormat = "KeepDateFormat";
        public static string Key_KeepDateAsNumber = "KeepDateAsNumber";
        public static string Command_Noop = "Noop";
        public static string Command_Message = "Message";
        public static string Command_Redirect = "Redirect";
        public static string Command_AppPage = "AppPage";
        public static string Command_ServerVM = "ServerVM";
        public static string Command_ServerData = "ServerData";
        public static string Command_BizException = "BizException";
        public static string Command_Exception = "Exception";
        //日期时间在Json中统一序列化为字符串，统一格式
        //public static String DateAsStringFormat = "yyyy-MM-dd HH:mm:ss";

        private bool isRcResult;
        private string command;
        private object resultdata;
        private object _extra;

        public bool IsErrorResult { get { return isRcResult == false; } }
        public object Extra { set => this._extra = value;  }
        //new { isRcResult, command = command ?? Command_Noop }
        public RichClientJsonResult(bool isRcResult, string command, object resultdata) : base(null)
        {
            this.isRcResult = isRcResult;
            this.command = command;
            this.resultdata = resultdata;
        }
        public override Task ExecuteResultAsync(ActionContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            
            if (command == Command_Message && resultdata == null)
            {
                resultdata = ValidationSummary(context);
            }
            if (_extra!=null)
            {
                Value = new { isRcResult, command, data = resultdata, extra = _extra };
            }
            else
            {
                Value = new { isRcResult, command, data = resultdata };
            }
            if (Value != null)
            {
                string str;
                if (!context.HttpContext.Request.Query.ContainsKey(Key_KeepDateFormat))
                {
                    str = JsonConvert.SerializeObject(Value, new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                        DateTimeZoneHandling =DateTimeZoneHandling.Local
                    });
                    //Netcore不再支持JavascriptSerializer. Newtonsoft如果设定日期格式为Microsoft
                    //输出的日期格式为/Date(数字+0800),需要用正则表达式将数字部分取出来
                    if (context.HttpContext.Request.Query.ContainsKey(Key_KeepDateAsNumber)) {//移动端只要数字
                        str = Regex.Replace(str, @"""\\/Date\((-?\d+)\+\d+\)\\/""", match =>
                        {
                            return match.Groups[1].Value.StartsWith("-")?"null": match.Groups[1].Value;
                        });
                    }
                    else
                    {//web端要字符串=(\Date(数字\
                        str = Regex.Replace(str, @"""\\(/Date\(-?\d+)\+\d+\)\\/""", match =>
                        {
                            return "\"" + match.Groups[1].Value + "\"";
                        });
                    }
                }
                else
                {//桌面等端要日期字符串
                    str = JsonConvert.SerializeObject(Value);
                }
                //Value = new { isRcResult, command, data = str };
                return response.WriteAsync(str);
            }

            return base.ExecuteResultAsync(context);
        }

        public string ValidationSummary(ActionContext context)
        {
            /******TODO:暂不提供服务器提供详细的验证错误信息
string messageSpan = null;
StringBuilder htmlSummary = new StringBuilder();
TagBuilder unorderedList = new TagBuilder("ul");

           var modelStates = context.ModelState.Values;

            foreach (var modelState in modelStates)
            {
                foreach (var modelError in modelState.Errors)
                {
                    string errorText = modelError.ErrorMessage;
                    if (string.IsNullOrEmpty(errorText) && modelError.Exception != null)
                    {
                        errorText = modelError.Exception.InnerException == null ?
                            modelError.Exception.Message :
                            modelError.Exception.InnerException.Message;
                    }
                    if (!string.IsNullOrEmpty(errorText))
                    {
                        TagBuilder listItem = new TagBuilder("li");
                        listItem.SetInnerText(errorText);
                        htmlSummary.Append(listItem.ToString(TagRenderMode.Normal));
                    }
                }
            }


            if (htmlSummary.Length == 0)
            {
                return "";
            }

            unorderedList.InnerHtml = htmlSummary.ToString();

            TagBuilder divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass((context.Controller.ViewData.ModelState.IsValid) ? HtmlHelper.ValidationSummaryValidCssClassName : HtmlHelper.ValidationSummaryCssClassName);
            divBuilder.InnerHtml = messageSpan + unorderedList.ToString(TagRenderMode.Normal);

            return divBuilder.ToString();
**********/
            return "RichClientJsonResult.ValidationSummary暂未实现";
        }

    }
}
