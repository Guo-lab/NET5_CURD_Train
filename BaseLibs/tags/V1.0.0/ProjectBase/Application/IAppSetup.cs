using System.Collections;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProjectBase.Application
{
    //所有类型应用都要做的初始化工作需要的环境数据
    public interface IAppSetup
    {
        IHostEnvironment Env { get; set; }
        IConfiguration Configuration { get; set; }
        IWindsorContainer WindsorContainer { get; set; }

        void ConfigureServices(IServiceCollection services);

    }
}
