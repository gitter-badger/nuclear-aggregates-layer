using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
	/// <summary>
	/// Contains the extensions for <see cref="IRepository{TEntity}"/> repository of <see cref="EntityToEntityReference"/>.
	/// </summary>
	public static class EntityToEntityRepositoryExtensions
	{
		/// <summary>
		/// Updates the references building the differences.
		/// </summary>
		public static void Update(this IRepository<EntityToEntityReference> repository,
		                          IEnumerable<EntityToEntityReference> oldReferences,
		                          IEnumerable<EntityToEntityReference> newReferences)
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}

			var oldRefs = (oldReferences ?? Enumerable.Empty<EntityToEntityReference>()).ToList();
			var newRefs = (newReferences ?? Enumerable.Empty<EntityToEntityReference>()).ToList();
			var removingLinks = oldRefs.Except(newRefs, EqualityComparer<EntityToEntityReference>.Default).ToList();
			var addingLinks = newRefs.Except(oldRefs, EqualityComparer<EntityToEntityReference>.Default).ToList();

			repository.AddRange(addingLinks);
			repository.DeleteRange(removingLinks);
			repository.Save();
		}
	}
}