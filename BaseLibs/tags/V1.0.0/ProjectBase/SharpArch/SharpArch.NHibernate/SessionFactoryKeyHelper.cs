namespace SharpArch.NHibernate
{
    using ProjectBase.Application;
    using SharpArch.Domain;

    public static class SessionFactoryKeyHelper
    {
        public static string GetKey()
        {
            var provider = CastleContainer.WindsorContainer.Resolve<ISessionFactoryKeyProvider>();
            return provider.GetKey();
        }

        public static string GetKeyFrom(object anObject)
        {
            ISessionFactoryKeyProvider provider;
            try
            {
                provider=CastleContainer.WindsorContainer.Resolve<ISessionFactoryKeyProvider>();
            }
            catch
            {
                provider = new DefaultSessionFactoryKeyProvider();
            }
            return provider.GetKeyFrom(anObject);
        }
    }
}