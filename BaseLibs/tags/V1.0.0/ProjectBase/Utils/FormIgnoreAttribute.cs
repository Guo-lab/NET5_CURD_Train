using System;

namespace ProjectBase.Utils
{
    [AttributeUsage( AttributeTargets.Property| AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class FormIgnoreAttribute : Attribute
    {
        public FormIgnoreAttribute ( )
        {
        }
    }
}