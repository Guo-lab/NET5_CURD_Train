using System;

namespace ProjectBase.AutoMapper
{
    /// <summary>
    /// 此接口的实现类将由框架自动创建映射(CreateMap), 接口方法Map将在AutoMapper缺省映射赋值完成后被调用
    /// </summary>
    public interface ISelfMapper<TSource>
    {
        void Map(TSource src)
        {
        }
        void Map(TSource src, BaseSelfMapperContext context)
        {

        }
    }
}
