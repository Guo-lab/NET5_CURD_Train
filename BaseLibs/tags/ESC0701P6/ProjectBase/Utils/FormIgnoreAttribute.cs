using System;

namespace ProjectBase.Utils
{
    /// <summary>
    /// 标记VM中属性不用于绑定客户端控件的验证规则。带此标记对应的表单项仍然会提交。
    /// </summary>
    [AttributeUsage( AttributeTargets.Property| AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class FormIgnoreAttribute : Attribute
    {
        public FormIgnoreAttribute ( )
        {
        }
    }
}