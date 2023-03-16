using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using ProjectBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.CastleWindsor
{
    public class RequirePropertyDependency : IContributeComponentModelConstruction
    {
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            model.Properties
                .Where(p=>p.Dependency.TargetType.FullName!=null
                                    && ProjectHierarchy.IocCheckNS.Any(ns=>p.Dependency.TargetType.FullName.StartsWith(ns)))
                .All(p => { 
                    p.Dependency.IsOptional = false; 
                    return true; 
                });
        }
    }
}
