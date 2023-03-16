using Castle.MicroKernel.Registration;
using E2biz.Utility.General;
using ESC5.Common;
using ESC5.Job.Message;
using IdentityService;
using Moq;
using ProjectBase.MessageSender;
using System;
using TestingBase.TestBase;
using TestingBase.TestDouble.IdentityService;

namespace ESC5.Test.BDIntegrateTest
{
    //测试Context启动过程尽量使用产品代码，如有不同，在子类中重写。
    //注意在CustomContainerRegister方法中添加必要的IOC注册
    public class TestAppCommonSetup : AppCommonSetup
    {
        protected override void CustomContainerRegister()
        {
            BDIntegrateTestContext.RegisterMemoryCache();

            AssemblyFilter allAssembly = new AssemblyFilter(System.AppDomain.CurrentDomain.BaseDirectory);

           WindsorContainer.Register(
               //可以用remoting或者rabbitMQ来处理需要定时执行或者可以异步执行（一般比较耗时）的任务
               //测试用模拟的类，实际不发送任何消息
               //Classes.FromAssemblyInDirectory(allAssembly)
               //    .BasedOn(typeof(IMessageSender<>))
               //    .WithService.AllInterfaces());
                Component.For<IIdentity>().ImplementedBy<MockIdentity>().LifestyleTransient()
            //   Component.For(typeof(IEmailMessageSender))
            //   .ImplementedBy(typeof(MessageSender.EmailMessageSender)));
            );

            //最后调用父类方法，则子类方法注册的类会作为默认实现覆盖父类方法中注册的类
            base.CustomContainerRegister();
        }
    }
}


