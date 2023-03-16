using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Utils
{
    /**
 * 标记字符串型输入属性对空格处理
 * @see --advanced
 */
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Parameter,Inherited =true)]
    public class TrimAttribute: Attribute
    {
        public bool Value { get; set; } = true;
        public TrimAttribute(bool trim)
        {
            Value = trim;
        }
    }
}
