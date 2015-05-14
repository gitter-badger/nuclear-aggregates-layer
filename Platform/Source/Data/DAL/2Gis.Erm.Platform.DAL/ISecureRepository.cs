using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Версия IRepository с проверкой безопасности.
    /// </summary>
    public interface ISecureRepository<in TEntity>
        where TEntity : class, IEntity, ICuratedEntity
    {
        /// <summary>
        /// Add the Entity Object to the Repository
        /// </summary>
        void Add(TEntity entity);
        
        /// <summary>
        /// Update changes made to the Entity object in the repository
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Delete the Entity object from the repository
        /// </summary>
        void Delete(TEntity entity);

        /// <summary>
        /// Persist the changes through the Add,Update,Delete to the persistence storage.
        /// </summary>
        int Save();
    }
}