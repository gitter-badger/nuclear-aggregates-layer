using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
	public static class ActivityObtainer
	{
		public static EntityToEntityReference ReferenceIfAny(ReferenceType regardingType, EntityName sourceEntityName, long sourceEntityId, EntityName targetEntityName, long? targetEntityId)
		{
			return targetEntityId.HasValue 
				       ? new EntityToEntityReference
					       {
						       ReferenceType = regardingType,
						       SourceEntityName = sourceEntityName,
						       SourceEntityId = sourceEntityId,
						       TargetEntityName = targetEntityName, 
						       TargetEntityId = targetEntityId.Value,
					       } 
				       : null;
		}
	}
}