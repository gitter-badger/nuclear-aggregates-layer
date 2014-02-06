using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class BankPropertiesConverter : DynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance>
    {
        protected override Bank CreateEntity(DictionaryEntityInstance dynamicEntityInstance)
        {
            return new Bank
                {
                    Id = dynamicEntityInstance.Id,
                    CreatedBy = dynamicEntityInstance.CreatedBy,
                    CreatedOn = dynamicEntityInstance.CreatedOn,
                    ModifiedBy = dynamicEntityInstance.ModifiedBy,
                    ModifiedOn = dynamicEntityInstance.ModifiedOn,
                    IsActive = dynamicEntityInstance.IsActive,
                    IsDeleted = dynamicEntityInstance.IsDeleted,
                    Timestamp = dynamicEntityInstance.Timestamp,
                };
        }

        protected override DictionaryEntityInstance CreateEntityInstance(Bank entity, long? referencedEntityId)
        {
            return new DictionaryEntityInstance
                {
                    Id = entity.Id,
                    EntityId = referencedEntityId,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    ModifiedBy = entity.ModifiedBy,
                    ModifiedOn = entity.ModifiedOn,
                    IsActive = entity.IsActive,
                    IsDeleted = entity.IsDeleted,
                    Timestamp = entity.Timestamp,
                };
        }

        protected override DictionaryEntityPropertyInstance CreateEntityPropertyInstace(long entityId, int propertyId)
        {
            return new DictionaryEntityPropertyInstance { PropertyId = propertyId };
        }
    }
}