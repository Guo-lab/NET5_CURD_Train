using System;
using Microsoft.AspNetCore.Http;

namespace ProjectBase.Web.Mvc
{
	public interface IFormTokenGenerator {
		string CreateFormToken(HttpRequest request,Type vmClass);
	}
}
