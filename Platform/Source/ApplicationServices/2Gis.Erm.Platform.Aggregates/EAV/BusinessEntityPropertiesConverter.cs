using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    // TODO {all, 07.04.2014}: какой-то перебор с маркерными интерфейсами у T 
    public class BusinessEntityPropertiesConverter<T> : DynamicEntityPropertiesConverter<T, BusinessEntityInstance, BusinessEntityPropertyInstance>
        where T : class, IEntity, IEntityPart, IEntityKey, IAuditableEntity, IDeactivatableEntity, IDeletableEntity, IStateTrackingEntity, new()
    {
        protected override T CreateEntity(BusinessEntityInstance dynamicEntityInstance)
        {
            return new T
                {
                    Id = dynamicEntityInstance.Id,
                    EntityId = dynamicEntityInstance.EntityId.Value,
                };
        }

        protected override BusinessEntityInstance CreateEntityInstance(T entity, long? referencedEntityId)
        {
            return new BusinessEntityInstance
                {
                    Id = entity.Id,
                    EntityId = referencedEntityId
                };
        }

        protected override BusinessEntityPropertyInstance CreateEntityPropertyInstace(long entityId, int propertyId)
        {
            return new BusinessEntityPropertyInstance
                {
                    EntityInstanceId = entityId,
                    PropertyId = propertyId
                };
        }
    }
}