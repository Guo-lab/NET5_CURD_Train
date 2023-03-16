using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.MetaData
{
    [Obsolete("即将删除，不要再使用，标准品里如有使用应尽快删除")]  
    public class VersionInfoAttribute:Attribute
    {
        private VersionInfoAttribute() { }
        public VersionInfoAttribute(string code,string version)
        {
            this.Code = code;
            this.Version = version;
        }
        public string Code { get; set; }
        public string Version { get; set; }
    }
}
