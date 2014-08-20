using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities
{
	public interface IUpdateRegardingObjectAggregateService<TEntity> : IUnknownAggregateSpecificOperation<AssignRegardingObjectIdentity>
		where TEntity : class, IEntity, IEntityKey
	{
		void ChangeRegardingObjects(IEnumerable<RegardingObject<TEntity>> oldReferences,
		                            IEnumerable<RegardingObject<TEntity>> newReferences);
	}
}