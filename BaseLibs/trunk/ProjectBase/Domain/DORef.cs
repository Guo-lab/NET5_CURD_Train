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
        public T ToReferencedDO(ICommonReader<T,TId> reader)
        {
            return Id==null || Id.Equals(default(TId)) ? null:reader.Get(Id);
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
