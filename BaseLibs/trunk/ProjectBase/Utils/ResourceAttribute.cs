using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Utils
{
    /// <summary>
    /// 标记IOC注入<br></br>
    ///@see --advanced
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Parameter,Inherited =true)]
    public class ResourceAttribute: Attribute
    {
        public string? Name { get; set; }
        public ResourceAttribute(string? name=null)
        {
            Name = name;
        }
    }
}
