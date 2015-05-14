using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class ActivityPropertiesConverter<TActivity> :
        DynamicEntityPropertiesConverter<TActivity, DictionaryEntityInstance, DictionaryEntityPropertyInstance>
        where TActivity : class, IEntity, IEntityKey, IAuditableEntity, ICuratedEntity, IDeactivatableEntity, IDeletableEntity, IStateTrackingEntity, new()
    {
        protected override TActivity CreateEntity(DictionaryEntityInstance entity)
        {
            return new TActivity { Id = entity.Id, OwnerCode = entity.OwnerCode };
        }

        protected override DictionaryEntityInstance CreateEntityInstance(TActivity entity, long? referencedEntityId)
        {
            return new DictionaryEntityInstance
            {
                Id = entity.Id,
                OwnerCode = entity.OwnerCode,
                EntityId = referencedEntityId,
            };
        }

        protected override DictionaryEntityPropertyInstance CreateEntityPropertyInstace(long entityId, int propertyId)
        {
            return new DictionaryEntityPropertyInstance
            {
                EntityInstanceId = entityId,
                PropertyId = propertyId
            };
        }
    }
}