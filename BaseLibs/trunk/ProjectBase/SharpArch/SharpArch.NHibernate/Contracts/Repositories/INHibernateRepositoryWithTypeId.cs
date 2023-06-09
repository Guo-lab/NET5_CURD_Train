namespace SharpArch.NHibernate.Contracts.Repositories
{
    using System;
    using System.Collections.Generic;

    using global::NHibernate;

    using SharpArch.Domain;
    using SharpArch.Domain.PersistenceSupport;

    public interface INHibernateRepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId>
    {
        #region Properties

        /// <summary>
        /// Provides a handle to application wide DB activities such as committing any pending changes,
        /// beginning a transaction, rolling back a transaction, etc.
        /// </summary>
        new IDbContext DbContext { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dissasociates the entity with the ORM so that changes made to it are not automatically 
        /// saved to the database.  More precisely, this removes the entity from <see cref="ISession" />'s cache.
        /// More details may be found at http://www.hibernate.org/hib_docs/nhibernate/html_single/#performance-sessioncache.
        /// </summary>
        void Evict(T entity);

        /// <summary>
        /// Looks for zero or more instances using the <see cref="IDictionary{string, object}"/> provided.
        /// The key of the collection should be the property name and the value should be
        /// the value of the property to filter by.
        /// </summary>
        [Obsolete("禁止使用")]
        IList<T> FindAll(IDictionary<string, object> propertyValuePairs);

        /// <summary>
        /// Looks for zero or more instances using the example provided.
        /// </summary>
        [Obsolete("禁止使用")]
        IList<T> FindAll(T exampleInstance, params string[] propertiesToExclude);

        /// <summary>
        /// Looks for a single instance using the property/values provided.
        /// </summary>
        /// <exception cref="NonUniqueResultException" />
        [Obsolete("禁止使用")]
        T FindOne(IDictionary<string, object> propertyValuePairs);

        /// <summary>
        /// Looks for a single instance using the example provided.
        /// </summary>
        /// <exception cref="NonUniqueResultException" />
       [Obsolete("禁止使用")]
        T FindOne(T exampleInstance, params string[] propertiesToExclude);

        /// <summary>
        /// Returns null if a row is not found matching the provided Id.
        /// </summary>
        T Get(TId Id, Enums.LockMode lockMode);

        /// <summary>
        /// Throws an exception if a row is not found matching the provided Id.
        /// </summary>
        T Load(TId Id);

        /// <summary>
        /// Throws an exception if a row is not found matching the provided Id.
        /// </summary>
        T Load(TId Id, Enums.LockMode lockMode);

        /// <summary>
        /// For entities that have assigned Id's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/nhibernate/html_single/#mapping-declaration-Id-assigned.
        /// </summary>
        T Save(T entity);

        /// <summary>
        /// For entities that have assigned Id's, you should explicitly call Update to update an existing one.
        /// Updating also allows you to commit changes to a detached object.  More info may be found at:
        /// http://www.hibernate.org/hib_docs/nhibernate/html_single/#manipulatingdata-updating-detached
        /// </summary>
        T Update(T entity);

        #endregion
    }
}