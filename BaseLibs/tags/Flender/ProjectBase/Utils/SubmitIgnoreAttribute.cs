using System;

namespace ProjectBase.Utils
{
    /// <summary>
    /// 标记VM中属性对应的客户端控件名值不提交
    /// </summary>
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class SubmitIgnoreAttribute : Attribute
    {
        public SubmitIgnoreAttribute ( )
        {
        }
    }
}