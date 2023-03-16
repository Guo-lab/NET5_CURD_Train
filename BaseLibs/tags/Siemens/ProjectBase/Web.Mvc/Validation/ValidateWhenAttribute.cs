using System;
namespace ProjectBase.Web.Mvc.Validation
{
    /// <summary>
    //  这是给客户端用的标记。 在VMInput的属性上加此标记，表示此属性参与条件验证的计算，是决定是否验证的条件因素，其控件blur时执行ShouldValidateGroups方法并根据结果执行验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateWhenAttribute : Attribute
    {
    }
}