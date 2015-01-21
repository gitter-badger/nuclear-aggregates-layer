using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class DictionaryEntityEntityPropertiesConverter<T> :
        DynamicEntityPropertiesConverter<T, DictionaryEntityInstance, DictionaryEntityPropertyInstance>
        where T : class, IEntity, IEntityKey, IAuditableEntity, IDeactivatableEntity, IDeletableEntity, IStateTrackingEntity, new()
    {
        protected override T CreateEntity(DictionaryEntityInstance dynamicEntityInstance)
        {
            return new T
                {
                    Id = dynamicEntityInstance.Id,
                };
        }

        protected override DictionaryEntityInstance CreateEntityInstance(T entity, long? referencedEntityId)
        {
            return new DictionaryEntityInstance
                {
                    Id = entity.Id,
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