namespace SharpArch.Domain.PersistenceSupport
{
    using System.Linq;

    /// <summary>
    /// Defines a LINQ implementation of the Repository Pattern that takes in a Specification to define
    /// the items that should be returned.
    /// </summary>
    public interface ILinqRepositoryWithTypedId<T, TId>
    {
        /// <summary>
        /// Delete the specified object from the repository
        /// </summary>
        /// <typeparam name="T">Type of entity to be deleted</typeparam>
        /// <param name="target">Entity to delete</param>
        void Delete(T target);

        /// <summary>
        /// Save the specified object to the repository
        /// </summary>
        /// <typeparam name="T">Type of entity to save</typeparam>
        /// <param name="entity">Entity to save</param>
        void Save(T entity);

        /// <summary>
        /// Saves  and evicts the specified object to the repository.
        /// from the session.
        /// </summary>
        /// <typeparam name="T">Type of Entity to Save / Evict</typeparam>
        /// <param name="entity">Entity to save and evict</param>
        void SaveAndEvict(T entity);

        /// <summary>
        /// Finds an item by Id.
        /// </summary>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <param name="Id">The Id of the entity</param>
        /// <returns>The matching item</returns>
        T FindOne(TId Id);

        /// <summary>
        /// Finds an item by a specification
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <returns>The the matching item</returns>
        //T FindOne(ILinqSpecification<T> specification);

        /// <summary>
        /// Finds all items within the repository.
        /// </summary>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <returns>All items in the repository</returns>
        IQueryable<T> FindAll();

        /// <summary>
        /// Finds all items by a specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <returns>All matching items</returns>
        //IQueryable<T> FindAll(ILinqSpecification<T> specification);
    }
}