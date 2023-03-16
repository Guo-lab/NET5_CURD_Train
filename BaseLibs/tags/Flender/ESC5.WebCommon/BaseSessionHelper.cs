using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using ProjectBase.Web.Mvc;
using System;
using System.Collections.Generic;

namespace ESC5.WebCommon
{
    /**
	 * 定义web session scope中保存的数据的key 以及管理这些key/value的相关方法.
	 * @author Rainy
	 *
	 */
    public abstract class BaseSessionHelper : IBaseSessionHelper
    {
        public static readonly string VAL_CODE_INFO = "ValCodeInfo";
        public static readonly string FORM_TOKEN = "FormToken";
        public static readonly string LOGIN_WRONG_CNT = "LoginWrongCnt";

        private static int maxParallelTokenCnt = 5; // 每用户最多5个并行表单token
     
        public bool HasFormToken(HttpRequest request, string vmTypeName, string token)
        {
            List<FormTokenStack> parallelTokens = request.HttpContext.Session.Get<List<FormTokenStack>>(FORM_TOKEN);
            if (parallelTokens == null || parallelTokens.Count == 0) return false;
            return parallelTokens.Exists((stack) => stack.HasToken(vmTypeName, token));
        }
        public bool HasFormToken(HttpRequest request)
        {
            List<FormTokenStack> parallelTokens = request.HttpContext.Session.Get<List<FormTokenStack>>(FORM_TOKEN);
            if (parallelTokens == null || parallelTokens.Count == 0) return false;
            return parallelTokens.Exists((stack) => stack.HasToken());
        }
        /// <summary>
        /// 需要客户端传ajax-nav参数才能支持栈(目前不支持)，否则栈中只有一个表单，所有表单都作为独立并行表单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vmTypeName"></param>
        /// <param name="token"></param>
        public void SaveFormToken(HttpRequest request, string vmTypeName, string token)
        {
            List<FormTokenStack> parallelTokens = request.HttpContext.Session.Get<List<FormTokenStack>>(FORM_TOKEN);
            if (parallelTokens == null)
            {
                parallelTokens = new List<FormTokenStack>();
            }
            else if (parallelTokens.Count > maxParallelTokenCnt)
            {
                parallelTokens.RemoveAt(0);
            }
            var stack = parallelTokens.Find(stack => stack.HasToken(vmTypeName));
            if (stack == null)
            {
                stack = new FormTokenStack();
                parallelTokens.Add(stack);
            }
            //TODO:string nav = request.Query[GlobalConstant.Key_For_AjaxNav];
            bool clear = true;// nav != null && nav.Equals(GlobalConstant.Value_For_AjaxNavRoot);
            stack.Save(vmTypeName, token, clear);
            request.HttpContext.Session.Set(FORM_TOKEN, parallelTokens);
        }

           public int GetLoginWrongCnt(HttpRequest request)
        {
            int? cnt = request.HttpContext.Session.GetInt32(LOGIN_WRONG_CNT);
            return cnt ?? 0;
        }

        public void AddLoginWrongCnt(HttpRequest request)
        {
            request.HttpContext.Session.SetInt32(LOGIN_WRONG_CNT, GetLoginWrongCnt(request) + 1);
        }

    }
}
