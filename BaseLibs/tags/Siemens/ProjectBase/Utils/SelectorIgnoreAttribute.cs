using System;

namespace ProjectBase.Utils
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SelectorIgnoreAttribute : Attribute
    {
    }
}

