using ESC5.Domain.DomainModel.TR;
using Microsoft.Extensions.DependencyInjection;
using ProjectBase.Application;
using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ESC5.Common
{
    public class AppCommonSetup : BaseAppCommonSetup
    {
        protected override void InitProjectHierarchy()
        {
            ProjectHierarchy.ProjectName = "ESC5";
            ProjectHierarchy.NamespaceMapToTablePrefix = GetNamespaceMapToTablePrefix();
            ProjectHierarchy.NonprefixMvcModuleNames = new string[] { };
            Pager.DefaultPageSize = 10;
            EnableDevTool();
        }

        protected override IDictionary<string, string> GetNamespaceMapToTablePrefix()
        {
            return new Dictionary<string, string> {
                {"TR","TR_" },
                {"RS","RS_" }
            };
        }

        protected override void CustomContainerRegister()
        {
            //无
        }
    }
}
