using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public class BranchOfficeOrganizationUnitPartPropertiesConverter :
        DynamicEntityPropertiesConverter<BranchOfficeOrganizationUnitPart, BusinessEntityInstance, BusinessEntityPropertyInstance>
    {
        protected override BranchOfficeOrganizationUnitPart CreateEntity(BusinessEntityInstance dynamicEntityInstance)
        {
            return new BranchOfficeOrganizationUnitPart
                {
                    Id = dynamicEntityInstance.Id
                };
        }

        protected override BusinessEntityInstance CreateEntityInstance(BranchOfficeOrganizationUnitPart entity, long? referencedEntityId)
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