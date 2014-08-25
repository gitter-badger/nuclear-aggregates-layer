using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	internal static class ActivityExtensions
	{
		public static RegardingObject<TEntity> ReferenceIfAny<TEntity>(this TEntity entity, EntityName targetEntityName, long? targetEntityId)
			where TEntity : class, IEntity, IEntityKey
		{
			return targetEntityId.HasValue
				       ? new RegardingObject<TEntity>
					       {
						       SourceEntityId = entity.Id,
						       TargetEntityName = targetEntityName,
						       TargetEntityId = targetEntityId.Value,
					       }
				       : null;
		}
	}
}