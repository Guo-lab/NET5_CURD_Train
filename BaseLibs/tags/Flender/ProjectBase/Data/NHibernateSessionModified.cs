
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using ProjectBase.Data.NHibernateMapByCode.Convention;
using ProjectBase.Domain;
using ProjectBase.Domain.NhibernateMapByCode;
using SharpArch.Domain;
using SharpArch.NHibernate;

namespace ProjectBase.Data
{
    /// <summary>
    /// SharpArch.NHibernate.NhibernateSession only uses FNH to configure and build session factory,we choose to change that to use NHibernate mapping by code instead.
    /// </summary>
    public static class NHibernateSessionModified
    {

        #region Properties 

        public static IDictionary<string, string> NamespaceMapToTablePrefix
        {
            get { return NamingConventions.NamespaceMapToTablePrefix; }
            set { NamingConventions.NamespaceMapToTablePrefix = value; }
        }
        public static Action<ModelMapper> AutoMappingOverride { set; get; }

        public static bool MapAllEnumsToStrings { set; get; }

        public static Type[] BaseEntityToIgnore { set; get; }

        public static string OutputXmlMappingsFile { set; get; }

        public static Dictionary<string, ISessionFactory> SessionFactories { set; get; }
        public static Action AfterInit { set; get; }

        #endregion Properties

        #region Methods 

        public static Configuration Init(
            ISessionStorage storage, string[] mappingAssemblies, string cfgFile, IDictionary<string, string> cfgProperties, string validatorCfgFile)
        {
            SessionFactories = new Dictionary<string, ISessionFactory>();
            NHibernateSession.InitStorage(storage);
            try
            {
                var cfg = AddConfiguration(NHibernateSession.DefaultFactoryKey, mappingAssemblies, cfgFile, cfgProperties, validatorCfgFile);
                if (AfterInit != null) AfterInit();
                return cfg;
            }
            catch (Exception)
            {
                // If this NHibernate config throws an exception, null the Storage reference so 
                // the config can be corrected without having to restart the web application.
                NHibernateSession.Storage = null;
                throw;
            }
        }
        public static void InitMultiDB(
    ISessionStorage storage, string[] mappingAssemblies, IDictionary<string, string> keyToCfgFiles)
        {
            SessionFactories = new Dictionary<string, ISessionFactory>();
            NHibernateSession.InitStorage(storage);
            foreach (var cfgFile in keyToCfgFiles)
            {
                var key = cfgFile.Key == "" ? NHibernateSession.DefaultFactoryKey : cfgFile.Key;
                try
                {
                    var cfg = AddConfiguration(key, mappingAssemblies, cfgFile.Value, new Dictionary<string, string>(), null);
                }
                catch (Exception)
                {
                    // If this NHibernate config throws an exception, null the Storage reference so 
                    // the config can be corrected without having to restart the web application.
                    NHibernateSession.Storage = null;
                    throw;
                }
            }
            if (AfterInit != null) AfterInit();
        }
        public static Configuration AddConfiguration(string factoryKey, string[] mappingAssemblies, string cfgFile, IDictionary<string, string> cfgProperties, string validatorCfgFile)
        {
            Configuration config;
            var configCache = NHibernateSession.ConfigurationCache;
            if (configCache != null)
            {
                config = configCache.LoadConfiguration(factoryKey, cfgFile, mappingAssemblies);
                if (config != null)
                {
                    var sessionFactory = config.BuildSessionFactory();
                    SessionFactories.Add(factoryKey, sessionFactory);
                    return NHibernateSession.AddConfiguration(factoryKey, sessionFactory, config, validatorCfgFile);
                }
            }

            config = AddConfiguration(factoryKey, mappingAssemblies,
                ConfigureNHibernate(cfgFile, cfgProperties),
                validatorCfgFile);

            if (configCache != null)
            {
                //TODO:暂不支持 configCache.SaveConfiguration(factoryKey, config);
            }

            return config;
        }

        public static Configuration AddConfiguration(string factoryKey, string[] mappingAssemblies, Configuration cfg, string validatorCfgFile)
        {
            var sessionFactory = CreateSessionFactoryFor(mappingAssemblies, cfg);
            Check.Require(
                !SessionFactories.ContainsKey(factoryKey),
                    "A session factory has already been configured with the key of " + factoryKey);
            SessionFactories.Add(factoryKey, sessionFactory);

            return NHibernateSession.AddConfiguration(factoryKey, sessionFactory, cfg, validatorCfgFile);
        }
        private static ISessionFactory CreateSessionFactoryFor(string[] mappingsAssemblies, Configuration cfg)
        {
            var assemblies = new List<Assembly>();
            Array.ForEach(mappingsAssemblies, i => assemblies.Add(Assembly.LoadFrom(MakeLoadReadyAssemblyName(i))));
            var mapping = GetMappings(assemblies);
            cfg.AddDeserializedMapping(mapping, null);
            return cfg.BuildSessionFactory();
        }
        private static Configuration ConfigureNHibernate(string cfgFile, IDictionary<string, string> cfgProperties)
        {
            var cfg = new Configuration();

            if (cfgProperties != null)
            {
                cfg.AddProperties(cfgProperties);
            }

            if (string.IsNullOrEmpty(cfgFile) == false)
            {
                cfg = cfg.Configure(cfgFile);
            }
            else if (File.Exists("Hibernate.cfg.xml"))
            {
                cfg = cfg.Configure();
            }
            foreach (var cfgProperty in cfgProperties)
            {
                cfg.Properties[cfgProperty.Key] = cfgProperty.Value;
            }
            return cfg;
        }

        private static bool IsDomainType(Type t)
        {
            if (t.IsInterface)
                return false;
            foreach (Type baseType in BaseEntityToIgnore)
            {
                if (baseType.IsAssignableFrom(t) || t == baseType)
                    return true;
            }
            return false;
        }

        private static HbmMapping GetMappings(IEnumerable<Assembly> mappingsAssemblies)
        {
            //Using the built-in auto-mapper
            var mapper = new ConventionModelMapper();
            DefineBaseClass(mapper);
            DefinePersistentProperty(mapper);
            var allEntities = new List<Type>();
            foreach (var mappingsAssembly in mappingsAssemblies)
            {
                allEntities.AddRange(mappingsAssembly.GetTypes().Where(
                        t => IsDomainType(t) && Attribute.GetCustomAttribute(t, typeof(MappingIgnoreAttribute), false) == null)
                    .ToList());
            }
            mapper.AddAllManyToManyRelations(allEntities);
            mapper.ApplyNamingConventions();
            if (MapAllEnumsToStrings) mapper.MapAllEnumsToStrings();
            if (AutoMappingOverride != null) AutoMappingOverride(mapper);
            OverrideByClassMapping(mapper, mappingsAssemblies);


            var mapping = mapper.CompileMappingFor(allEntities);

            //ShowOutputXmlMappings(mapping);
            return mapping;
        }

        private static bool IsRoot(Type type)
        {
            return type != null && type.BaseType != null && (BaseEntityToIgnore.Contains(type.BaseType) ||
                                Attribute.GetCustomAttribute(type.BaseType, typeof(MappingIgnoreAttribute), false) != null);
        }
        private static void DefineBaseClass(ConventionModelMapper mapper)
        {
            if (BaseEntityToIgnore == null)
            {
                BaseEntityToIgnore = new Type[] { 
                    typeof(BaseDomainObject),
                    typeof(BaseDomainObjectWithSeqId),
                    typeof(DomainObjectWithAssignedIntId),
                    typeof(DomainObjectWithAssignedGuidId)};
            }
            mapper.IsEntity((type, declared) => IsDomainType(type));
            mapper.IsRootEntity((type, declared) => IsRoot(type));
            //mapper.IsTablePerClassHierarchy((type, declared) =>true);
        }
        private static void DefinePersistentProperty(ConventionModelMapper mapper)
        {
            mapper.IsPersistentProperty((mi, declared) => mi.GetCustomAttribute<MappingIgnoreAttribute>() == null && mi.Name != "RowVersion");
        }

        private static void ShowOutputXmlMappings(HbmMapping mapping)
        {
            var outputXmlMappings = mapping.AsString();
            Console.WriteLine(outputXmlMappings);
            File.WriteAllText(OutputXmlMappingsFile, outputXmlMappings);
        }
        private static string MakeLoadReadyAssemblyName(string assemblyName)
        {
            return (assemblyName.IndexOf(".dll") == -1) ? assemblyName.Trim() + ".dll" : assemblyName.Trim();
        }

        private static void OverrideByClassMapping(ModelMapper mapper, IEnumerable<Assembly> mappingsAssemblies)
        {
            var allMappingClasses = new List<Type>();
            foreach (var mappingsAssembly in mappingsAssemblies)
            {
                allMappingClasses.AddRange(mappingsAssembly.GetTypes().Where(
                        t => typeof(IClassByClassMapping).IsAssignableFrom(t)
                    ).ToList());
            }
            mapper.AddMappings(allMappingClasses);
        }


        #endregion Methods
    }
}