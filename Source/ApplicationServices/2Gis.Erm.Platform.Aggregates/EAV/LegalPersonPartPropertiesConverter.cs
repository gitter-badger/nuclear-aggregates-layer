using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public class LegalPersonPartPropertiesConverter : DynamicEntityPropertiesConverter<LegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance>
    {
        protected override LegalPersonPart CreateEntity(BusinessEntityInstance dynamicEntityInstance)
        {
            return new LegalPersonPart
                {
                    Id = dynamicEntityInstance.Id
                };
        }

        protected override BusinessEntityInstance CreateEntityInstance(LegalPersonPart entity, long? referencedEntityId)
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