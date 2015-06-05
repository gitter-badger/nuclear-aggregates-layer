using System.Collections.Generic;

namespace NuClear.Storage.Writings
{
    /// <summary>
    /// Represents the non-generic repository contract.
    /// </summary>
    public interface IRepository
    {
    }

    /// <summary>
    /// Generic Repository Pattern Interface
    /// </summary>
    public interface IRepository<in TEntity> : IRepository where TEntity : class
    {
        /// <summary>
        /// Add the Entity Object to the Repository
        /// </summary>
        /// <param name="entity">The Entity object to add</param>
        void Add(TEntity entity);

        /// <summary>
        /// Add range of the Entity Objects to the Repository
        /// </summary>
        /// <param name="entities">The Entity objects to add</param>
        void AddRange(IEnumerable<TEntity> entities);
        
        /// <summary>
        /// Update changes made to the Entity object in the repository
        /// </summary>
        /// <param name="entity">The Entity object to update</param>
        void Update(TEntity entity);

        /// <summary>
        /// Delete the Entity object from the repository
        /// </summary>
        /// <param name="entity">The Entity object to delete</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete range of the Entity objects from the repository
        /// </summary>
        /// <param name="entities">The Entity objects to delete</param>
        void DeleteRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Persist the changes through the Add, Update, Delete to the persistence storage.
        /// </summary>
        /// <returns>The number of successful records persisted in the persistence storage</returns>
        int Save();
    }
}