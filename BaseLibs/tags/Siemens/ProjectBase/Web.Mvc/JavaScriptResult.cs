using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectBase.Web.Mvc
{
    public class JavaScriptResult : ContentResult {
        public JavaScriptResult(string script)
        {
            this.Content = script;
            this.ContentType = "application/x-javascript;charset=utf-8";
        }
    }
}
