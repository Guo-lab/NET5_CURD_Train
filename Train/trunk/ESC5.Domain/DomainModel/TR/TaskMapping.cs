using ESC5.Domain.DomainModel.TR;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using ProjectBase.Domain.NhibernateMapByCode;

namespace ESC5.Domain.DomainModel.RS
{
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


