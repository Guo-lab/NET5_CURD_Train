using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Utils
{
    /// <summary>
    /// 标记类对应的模块名。一般类对应的模块名为命名空间的最后一段。如果需要特殊的指定，就使用此标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited =false)]
    public class PBAreaAttribute: Attribute
    {
        public string Value { get; set; }
        public PBAreaAttribute(string value)
        {
            Value = value;
        }
    }
}
