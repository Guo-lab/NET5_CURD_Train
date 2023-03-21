using ESC5.Domain.DomainModel.TR;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using ProjectBase.Domain.NhibernateMapByCode;





namespace ESC5.Domain.DomainModel.RS
{
    /// <summary>
    /// 可编写Mapping class来定制映射
    //// 1.类名为xxxMapping,其中xxx为DomainObject类名.
    //// 2.继承ClassMapping基类，IClassByClassMapping接口
    //// 3.仅对不符合自动映射的部分进行定制。无法自动映射的需手动配置，定制映射优先于自动映射。
    //// 4.自动映射：根据命名规范一般不用配置hbm就可自动映射（表名、字段名、单向多对一、双向一对多）的双向一对多中没有设置级联，因此父子关系需要的级联设置需定制映射
    /// </summary>
    public class TaskMapping : ClassMapping<Task>, IClassByClassMapping
    {
        public TaskMapping()
        {
            //ManyToOne(o => o.User, m =>
            //{
            //    //  m.Column("UserId");
            //    //m.Lazy(LazyRelation.Proxy);
            //});
        }
    }
}


