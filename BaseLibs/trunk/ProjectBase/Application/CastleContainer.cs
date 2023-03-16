using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;

namespace ProjectBase.Application
{
    /// <summary>
    /// 为移植兼容性保留，不符合IOC，以后可能会废弃
    /// </summary>
    [Obsolete]
    public class CastleContainer
    {
        public static IWindsorContainer WindsorContainer { get; set; }
    }
}
