using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectBase.Domain
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false,Inherited=true)]
    public class RefTextAttribute : Attribute
    {
        public string Formula { get; set; }
        /// <summary>
        /// 不指定formula时，表示用标记所在的属性字段作为RefText属性值来源
        /// </summary>
        public RefTextAttribute()
        {

        }
        /// <summary>
        /// 指定formula时,此标记可标在任意一个属性上，效果一样。
        /// </summary>
        /// <param name="formula">字符串内容是sql表达式</param>
        public RefTextAttribute(string formula)
        {
            Formula = formula;
        }
    }
}
