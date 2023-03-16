using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.MetaData
{
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
