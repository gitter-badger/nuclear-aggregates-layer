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
		public static void Update<T>(this IRepository<RegardingObject<T>> repository,
								  IEnumerable<RegardingObject<T>> oldReferences,
								  IEnumerable<RegardingObject<T>> newReferences)
			where T : class, IEntity, IEntityKey
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}

			var oldRefs = (oldReferences ?? Enumerable.Empty<RegardingObject<T>>()).ToList();
			var newRefs = (newReferences ?? Enumerable.Empty<RegardingObject<T>>()).ToList();
			var removingLinks = oldRefs.Except(newRefs, EqualityComparer<RegardingObject<T>>.Default).ToList();
			var addingLinks = newRefs.Except(oldRefs, EqualityComparer<RegardingObject<T>>.Default).ToList();

			repository.AddRange(addingLinks);
			repository.DeleteRange(removingLinks);
			repository.Save();
		}
	}
}