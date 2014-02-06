using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public abstract class DynamicEntityPropertiesConverter<TEntity, TEntityInstance, TEntityPropertyInstace> :
        IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TEntityPropertyInstace>
        where TEntity : class, IEntity 
        where TEntityInstance : class, IDynamicEntityInstance
        where TEntityPropertyInstace : class, IDynamicEntityPropertyInstance
    {
        private readonly Action<TEntity, ICollection<TEntityPropertyInstace>> _converterFromPersistence;
        private readonly Action<TEntity, long, ICollection<TEntityPropertyInstace>> _converterToPersistence;

        protected DynamicEntityPropertiesConverter()
        {
            var propertyIdentities = DynamicEntityMetadataRegistry.GetPropertyIdentities<TEntity>();
            var entityPropertyIdentities = propertyIdentities as IEntityPropertyIdentity[] ?? propertyIdentities.ToArray();

            _converterFromPersistence = entityPropertyIdentities.BuildValueGettersExpression<TEntity, TEntityPropertyInstace>(GetPropertyValue).Compile();
            _converterToPersistence = entityPropertyIdentities.BuildValueSettersExpression<TEntity, TEntityPropertyInstace>(SetPropertyValue).Compile();
        }

        public TEntity ConvertFromDynamicEntityInstance(TEntityInstance dynamicEntityInstance, ICollection<TEntityPropertyInstace> propertyInstances)
        {
            var entity = CreateEntity(dynamicEntityInstance);
            _converterFromPersistence(entity, propertyInstances);
            return entity;
        }

        public Tuple<TEntityInstance, ICollection<TEntityPropertyInstace>> ConvertToDynamicEntityInstance(
            TEntity entity,
            ICollection<TEntityPropertyInstace> propertyInstances, 
            long? referencedEntityId)
        {
            var entityInstance = CreateEntityInstance(entity, referencedEntityId);
            _converterToPersistence(entity, entityInstance.Id, propertyInstances);

            return Tuple.Create(entityInstance, propertyInstances);
        }

        protected abstract TEntity CreateEntity(TEntityInstance dynamicEntityInstance);
        protected abstract TEntityInstance CreateEntityInstance(TEntity entity, long? referencedEntityId);
        protected abstract TEntityPropertyInstace CreateEntityPropertyInstace(long entityId, int propertyId);

        private static object GetPropertyValue(IEnumerable<TEntityPropertyInstace> propertyInstances, int propertyId, Type propertyType)
        {
            var property = propertyInstances.FirstOrDefault(x => x.PropertyId == propertyId);
            var getter = DynamicEntityPropertyMapper<TEntityPropertyInstace>.GetGetter(propertyType);
            return getter(property);
        }

        private void SetPropertyValue(ICollection<TEntityPropertyInstace> propertyInstances,
                                      int propertyId,
                                      Type propertyType,
                                      object propertyValue,
                                      long entityId)
        {
            var property = propertyInstances.FirstOrDefault(x => x.PropertyId == propertyId);
            if (property == null)
            {
                property = CreateEntityPropertyInstace(entityId, propertyId);
                propertyInstances.Add(property);
            }

            var setter = DynamicEntityPropertyMapper<TEntityPropertyInstace>.GetSetter(propertyType);
            setter(property, propertyValue);
        }
    }
}