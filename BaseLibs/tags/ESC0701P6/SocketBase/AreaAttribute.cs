using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    /// <summary>
    /// 同MVC 的Area标记
    /// </summary>
    public class AreaAttribute:Attribute
    {
        public string Value { get; set; }
        public AreaAttribute(string value)
        {
            Value = value;
        }
    }
}
