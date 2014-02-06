using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class LegalPersonProfilePartPropertiesConverter : DynamicEntityPropertiesConverter<LegalPersonProfilePart, BusinessEntityInstance, BusinessEntityPropertyInstance>
    {
        protected override LegalPersonProfilePart CreateEntity(BusinessEntityInstance dynamicEntityInstance)
        {
            return new LegalPersonProfilePart
                {
                    Id = dynamicEntityInstance.Id
                };
        }

        protected override BusinessEntityInstance CreateEntityInstance(LegalPersonProfilePart entity, long? referencedEntityId)
        {
            return new BusinessEntityInstance
                {
                    Id = entity.Id,
                    EntityId = referencedEntityId
                };
        }

        protected override BusinessEntityPropertyInstance CreateEntityPropertyInstace(long entityId, int propertyId)
        {
            return new BusinessEntityPropertyInstance { PropertyId = propertyId };
        }
    }
}