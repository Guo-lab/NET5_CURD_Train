using System;

namespace ProjectBase.Utils
{
    /// <summary>
    /// ���VM�����Զ�Ӧ�Ŀͻ��˿ؼ���ֵ���ύ
    /// </summary>
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class SubmitIgnoreAttribute : Attribute
    {
        public SubmitIgnoreAttribute ( )
        {
        }
    }
}