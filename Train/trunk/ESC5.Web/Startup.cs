using Castle.Windsor;
using ESC5.Common;
using ESC5.Web.Mvc.Shared;
using Microsoft.Extensions.Configuration;
using ProjectBase.Web.Mvc;

namespace ESC5.Web
{
    public class Startup : BaseMvcApplication
    {
        public Startup(IConfiguration config) : base(config)
        {
            AppCommonSetup = new AppCommonSetup();
            AppCommonSetup.Configuration = config;
            AppSpecialSetup = new WebAppSpecialSetup();
            AppSpecialSetup.Configuration = config;
        }
    }
}
