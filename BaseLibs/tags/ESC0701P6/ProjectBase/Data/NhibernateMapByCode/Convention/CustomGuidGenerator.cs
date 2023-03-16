using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.ByCode;
using NHibernate.Persister.Entity;

namespace ProjectBase.Data.NHibernateMapByCode.Convention
{
    public class CustomGuidGenerator : NHibernate.Id.GuidGenerator, NHibernate.Id.IIdentifierGenerator 

    {
        public new object Generate(NHibernate.Engine.ISessionImplementor session, object obj)
        {
            IEntityPersister persister = session.GetEntityPersister("",obj);
            // Determine if an ID has been assigned. 
            object Id = persister.GetIdentifier(obj);
            if (Id != null && ((Guid)Id).ToString() != Guid.Empty.ToString ()) return Id;
            return base.Generate(session, obj);
        }
    }

    public class GuidGeneratorDef : IGeneratorDef
    {
        public string Class
        {
            get { return typeof(CustomGuidGenerator).AssemblyQualifiedName; }
        }

        public object Params
        {
            get { return null; }
        }

        public Type DefaultReturnType
        {
            get { return typeof(Guid); }
        }

        public bool SupportedAsCollectionElementId
        {
            get { return true; }
        }
    }

    public static class CustomGenerators
    {
        static CustomGenerators()
        {
            GuidGenerator = new GuidGeneratorDef();
        }
        public static IGeneratorDef GuidGenerator  { get; private set; }
    }
}
