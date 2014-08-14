using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
	public static class EntityToEntityReferenceExtensions
	{
		public static EntityReference Lookup(this IEnumerable<EntityToEntityReference> references, ReferenceType referenceType, EntityName entityName, Func<long, string> getName)
		{
			return (references ?? Enumerable.Empty<EntityToEntityReference>())
				.Where(x => x.ReferenceType == referenceType && x.TargetEntityName == entityName)
				.Select(x => new EntityReference { Id = x.TargetEntityId, Name = getName(x.TargetEntityId) })
				.SingleOrDefault();
		}
	}
}