using System;

namespace ProjectBase.Web.Mvc
{
    /**
 * 显式标记一个类为VM输入类。
 * @author Rainy
 * @see --advanced
 */
    [AttributeUsage(AttributeTargets.Class,AllowMultiple= false)]
    public class VmAsInputAttribute :Attribute 
    {
    }
}
