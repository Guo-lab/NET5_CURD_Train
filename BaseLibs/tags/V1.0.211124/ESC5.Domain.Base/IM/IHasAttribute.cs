using System.Collections.Generic;

namespace ESC5.Domain.Base.IM
{
    //public interface IHasAttribute
    //{
    //    void AddFixedAttribute(string attributeName, string attributeValue);
    //    void AddVariantAttribute(string attributeName,bool IsRequired);

    //    void ClearAttributes();
    //}
    public interface IHasAttribute<T> where T : IPartAttribute
    {
        IList<T> Attributes { get; set; }
        void AddAttribute(T attr);
    }

    public static class AttributeExt
    {
        /// <summary>
        /// 将一个单据的属性赋值到另外一个单据
        /// </summary>
        /// <typeparam name="SourceAttributeT"></typeparam>
        /// <typeparam name="DestAttributeT"></typeparam>
        /// <param name="sourceOrder">被复制的单据</param>
        /// <param name="destOrder">目标单据</param>
        public static void CopyAttributeTo<SourceAttributeT, DestAttributeT>(
            this IHasAttribute<SourceAttributeT> sourceOrder, IHasAttribute<DestAttributeT> destOrder)
            where SourceAttributeT : IPartAttribute
            where DestAttributeT : IPartAttribute, new()
        {
            if (destOrder.Attributes != null) { destOrder.Attributes.Clear(); }
            if (sourceOrder.Attributes != null)
            {
                foreach (SourceAttributeT attr in sourceOrder.Attributes)
                {
                    destOrder.AddAttribute(new DestAttributeT
                    {
                        AttributeId = attr.AttributeId,
                        PartAttrName = attr.PartAttrName,
                        PartAttrValue = attr.PartAttrValue,
                        IsFixed = attr.IsFixed,
                        IsRequired = attr.IsRequired
                    });
                }
            }
        }
    }
    //public interface IHasAttribute<T>:IHasAttribute where T : PartAttrBase
    //{
    //    IList<T> Attributes { get; set; }
    //}
}
