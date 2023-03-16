using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectBase.Domain;

namespace ProjectBase.Domain
{
    //public class DORef<TId>
    //{
        
    //}
    public class DORef<T,TId> where T : BaseDomainObjectWithTypedId<TId>
                              where TId: struct 
    {
        public TId? Id { get; set; }
        public string RefText { get; set; }
        public T ToReferencedDO(ICommonBD<T,TId> bd)
        {
            return bd.Get(Id);
        }

        public DORef()
        {
        }
        public DORef(TId id)
        {
            Id = id;
        }
        public DORef(TId id,string refText)
        {
            Id = id;
            RefText = refText;
        }
        public DORef(T entity)
        {
            Id = entity.Id;
            RefText = entity.RefText;
        }

        public bool IsNull()
        {
            return Id == null;
        }

    }

}
