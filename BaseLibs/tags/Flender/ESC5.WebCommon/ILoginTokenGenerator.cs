using IdentityService;
using Microsoft.AspNetCore.Http;

namespace ESC5.WebCommon
{
	public interface ILoginTokenGenerator
	{
		/**
		 * 创建登录令牌及相关工作（包括服务器端session存根信息和下发令牌到客户端等）
		 * @param request
		 * @param response
		 * @param loginInfo
		 * @return
		 */
		LoginToken CreateLoginToken(HttpRequest request, HttpResponse response, int userId);
		/**
		 * 删除cookie中登录令牌相关信息。指退出会话时对客户端进行的清理工作。
		 * @param request
		 * @param response
		 */
		void RemoveLoginToken(HttpRequest request, HttpResponse response);
	}
}
