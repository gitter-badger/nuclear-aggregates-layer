using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class BankPropertiesConverter : DynamicEntityPropertiesConverter<Bank, DictionaryEntityInstance, DictionaryEntityPropertyInstance>
    {
        protected override Bank CreateEntity(DictionaryEntityInstance dynamicEntityInstance)
        {
            return new Bank
                {
                    Id = dynamicEntityInstance.Id
                };
        }

        protected override DictionaryEntityInstance CreateEntityInstance(Bank entity, long? referencedEntityId)
        {
            return new DictionaryEntityInstance
                {
                    Id = entity.Id,
                    EntityId = referencedEntityId
                };
        }

        protected override DictionaryEntityPropertyInstance CreateEntityPropertyInstace(long entityId, int propertyId)
        {
            return new DictionaryEntityPropertyInstance { PropertyId = propertyId };
        }
    }
}