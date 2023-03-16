using ESC5.WebCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Web.Mvc;

namespace ESC5.Web.Mvc.Shared
{
    public class WebAppSpecialSetup : BaseWebAppSpecialSetup
    {
        protected override void ConfigMvc(IServiceCollection services)
        {
            base.ConfigMvc(services);
            services.AddSingleton<AppBaseControllerExceptionFilter>();
            services.Configure<MvcOptions>((o) =>
            {
                o.Filters.Add(typeof(AppBaseControllerExceptionFilter));
            });
        }
    }

}