﻿using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Generic Repository Pattern Interface
    /// </summary>
    public interface IRepository<in TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Add the Entity Object to the Repository
        /// </summary>
        /// <param name="entity">The Entity object to add</param>
        void Add(TEntity entity);
        
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
        /// Persist the changes through the Add,Update,Delete to the persistence storage.
        /// </summary>
        /// <returns>The number of successful records persisted in the persistence storage</returns>
        int Save();
    }
}