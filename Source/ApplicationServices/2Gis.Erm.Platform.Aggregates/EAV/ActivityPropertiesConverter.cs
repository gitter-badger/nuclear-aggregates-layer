using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class ActivityPropertiesConverter<TActivity> : DynamicEntityPropertiesConverter<TActivity, ActivityInstance, ActivityPropertyInstance> 
        where TActivity : ActivityBase, new()
    {
        protected override TActivity CreateEntity(ActivityInstance dynamicEntityInstance)
        {
            return new TActivity
                {
                    Id = dynamicEntityInstance.Id,
                    ClientId = dynamicEntityInstance.ClientId,
                    ContactId = dynamicEntityInstance.ContactId,
                    FirmId = dynamicEntityInstance.FirmId,
                    CreatedBy = dynamicEntityInstance.CreatedBy,
                    CreatedOn = dynamicEntityInstance.CreatedOn,
                    ModifiedBy = dynamicEntityInstance.ModifiedBy,
                    ModifiedOn = dynamicEntityInstance.ModifiedOn,
                    IsActive = dynamicEntityInstance.IsActive,
                    IsDeleted = dynamicEntityInstance.IsDeleted,
                    OwnerCode = dynamicEntityInstance.OwnerCode,
                    Timestamp = dynamicEntityInstance.Timestamp,
                    Type = (ActivityType)dynamicEntityInstance.Type
                };
        }

        protected override ActivityInstance CreateEntityInstance(TActivity entity, long? referencedEntityId)
        {
            return new ActivityInstance
                {
                    Id = entity.Id,
                    ClientId = entity.ClientId,
                    ContactId = entity.ContactId,
                    FirmId = entity.FirmId,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    ModifiedBy = entity.ModifiedBy,
                    ModifiedOn = entity.ModifiedOn,
                    IsActive = entity.IsActive,
                    IsDeleted = entity.IsDeleted,
                    OwnerCode = entity.OwnerCode,
                    Timestamp = entity.Timestamp,
                    Type = (int)entity.Type
                };
        }

        protected override ActivityPropertyInstance CreateEntityPropertyInstace(long entityId, int propertyId)
        {
            return new ActivityPropertyInstance { ActivityId = entityId, PropertyId = propertyId };
        }
    }
}