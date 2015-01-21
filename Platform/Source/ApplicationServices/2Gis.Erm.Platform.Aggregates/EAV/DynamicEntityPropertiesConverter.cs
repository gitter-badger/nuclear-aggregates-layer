using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    // TODO {all, 07.04.2014}: какой-то перебор с маркерными интерфейсами у TEntity возможно лучше запиливать, как набор mapperstrategy: Auditable, Deactivatable и т.п.
    public abstract class DynamicEntityPropertiesConverter<TEntity, TEntityInstance, TEntityPropertyInstace> :
        IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TEntityPropertyInstace>
        where TEntity : class, IEntity, IEntityKey, IAuditableEntity, IDeactivatableEntity, IDeletableEntity, IStateTrackingEntity
        where TEntityInstance : class, IDynamicEntityInstance
        where TEntityPropertyInstace : class, IDynamicEntityPropertyInstance
    {
        private readonly Action<TEntity, ICollection<TEntityPropertyInstace>> _converterFromPersistence;
        private readonly Action<TEntity, long, ICollection<TEntityPropertyInstace>, BusinessModel> _converterToPersistence;

        protected DynamicEntityPropertiesConverter()
        {
            _converterFromPersistence = DynamicEntityMetadataRegistry.GetPropertyIdentities<TEntity>()
                                                                     .BuildValueGettersExpression<TEntity, TEntityPropertyInstace>(GetPropertyValue)
                                                                     .Compile();
            _converterToPersistence = DynamicEntityMetadataRegistry.GetPropertyIdentities<TEntity>()
                                                                   .BuildValueSettersExpression<TEntity, TEntityPropertyInstace>(SetPropertyValue)
                                                                   .Compile();
        }

        public TEntity ConvertFromDynamicEntityInstance(TEntityInstance dynamicEntityInstance, ICollection<TEntityPropertyInstace> propertyInstances)
        {
            var entity = CreateEntity(dynamicEntityInstance);
            _converterFromPersistence(entity, propertyInstances);

            SetAuditableProperties(dynamicEntityInstance, entity);
            SetDeactivatableProperties(dynamicEntityInstance, entity);
            SetDeletableProperties(dynamicEntityInstance, entity);
            SetDeletableProperties(dynamicEntityInstance, entity);
            SetStateTrackingEntityProperties(dynamicEntityInstance, entity);

            return entity;
        }

        public Tuple<TEntityInstance, ICollection<TEntityPropertyInstace>> ConvertToDynamicEntityInstance(
            TEntity entity,
            ICollection<TEntityPropertyInstace> propertyInstances, 
            long? referencedEntityId)
        {
            var businessModel = typeof(TEntity).IsAdapted() ? typeof(TEntity).AsAdapted().AsBusinessModel() : BusinessModel.NotSet;
            var dynamicEntityInstance = CreateEntityInstance(entity, referencedEntityId);
            _converterToPersistence(entity, dynamicEntityInstance.Id, propertyInstances, businessModel);

            SetAuditableProperties(entity, dynamicEntityInstance);
            SetDeactivatableProperties(entity, dynamicEntityInstance);
            SetDeletableProperties(entity, dynamicEntityInstance);
            SetStateTrackingEntityProperties(entity, dynamicEntityInstance);

            return Tuple.Create(dynamicEntityInstance, propertyInstances);
        }

        protected abstract TEntity CreateEntity(TEntityInstance dynamicEntityInstance);
        protected abstract TEntityInstance CreateEntityInstance(TEntity entity, long? referencedEntityId);
        protected abstract TEntityPropertyInstace CreateEntityPropertyInstace(long entityId, int propertyId);

        private static void SetAuditableProperties(IAuditableEntity source, IAuditableEntity target)
        {
            target.CreatedBy = source.CreatedBy;
            target.CreatedOn = source.CreatedOn;
            target.ModifiedBy = source.ModifiedBy;
            target.ModifiedOn = source.ModifiedOn;
        }

        private static void SetDeactivatableProperties(IDeactivatableEntity source, IDeactivatableEntity target)
        {
            target.IsActive = source.IsActive;
        }

        private static void SetDeletableProperties(IDeletableEntity source, IDeletableEntity target)
        {
            target.IsDeleted = source.IsDeleted;
        }

        private static void SetStateTrackingEntityProperties(IStateTrackingEntity source, IStateTrackingEntity target)
        {
            target.Timestamp = source.Timestamp;
        }
        
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