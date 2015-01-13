using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    /// <summary>
    /// Contains the extensions for <see cref="IRepository{TEntity}"/> repository of <see cref="EntityReference{TAggregate}"/>.
    /// </summary>
    internal static class EntityToEntityRepositoryExtensions
    {
        /// <summary>
        /// Updates the references building the differences.
        /// </summary>
        public static void Update<TEntity, TEntityReference>(this IRepository<TEntityReference> repository,
                                                             IEnumerable<TEntityReference> oldReferences,
                                                             IEnumerable<TEntityReference> newReferences)
            where TEntity : IEntity
            where TEntityReference : EntityReference<TEntity>, IEntity
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            var oldRefs = (oldReferences ?? Enumerable.Empty<TEntityReference>()).Where(x => x != null).ToArray();
            var newRefs = (newReferences ?? Enumerable.Empty<TEntityReference>()).Where(x => x != null).ToArray();
            var removingLinks = oldRefs.Except(newRefs, EqualityComparer<TEntityReference>.Default).ToArray();
            var addingLinks = newRefs.Except(oldRefs, EqualityComparer<TEntityReference>.Default).ToArray();

            repository.AddRange(addingLinks);
            repository.DeleteRange(removingLinks);
            repository.Save();
        }

        /// <summary>
        /// Updates the references building the differences.
        /// </summary>
        public static void Update<TEntity, TEntityReference>(this IRepository<TEntityReference> repository,
                                                             TEntityReference oldReference,
                                                             TEntityReference newReference)
            where TEntity : IEntity
            where TEntityReference : EntityReference<TEntity>, IEntity
        {
           
            Update<TEntity,TEntityReference>(repository,new []{oldReference},new []{newReference});
          
        }
    }
}