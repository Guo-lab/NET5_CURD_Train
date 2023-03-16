using ProjectBase.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace ESC5.Web.Mvc.Home
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            throw LocateViewOnly;
        }
        public IActionResult MainFrame()
        {
            //特殊用法
            return ForView(new
            {
                Greeting = "Hello World!"
            });
        }
    }
}
