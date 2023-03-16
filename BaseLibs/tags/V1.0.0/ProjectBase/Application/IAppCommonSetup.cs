using System.Collections;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.Extensions.Hosting;

namespace ProjectBase.Application
{
    /// <summary>
    ///     所有类型应用都要做的初始化工作
    /// </summary>
    public interface IAppCommonSetup : IAppSetup
    {
        void SetupCommonFeature();
        NHibernate.Cfg.Configuration InitializeNHibnerate(SharpArch.NHibernate.ISessionStorage storage);
    }
}
