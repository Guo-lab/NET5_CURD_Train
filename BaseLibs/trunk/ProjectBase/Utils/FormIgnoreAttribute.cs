using System;

namespace ProjectBase.Utils
{
    /// <summary>
    /// ���VM�����Բ����ڰ󶨿ͻ��˿ؼ�����֤���򡣴��˱�Ƕ�Ӧ�ı�����Ȼ���ύ��
    /// </summary>
    [AttributeUsage( AttributeTargets.Property| AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class FormIgnoreAttribute : Attribute
    {
        public FormIgnoreAttribute ( )
        {
        }
    }
}