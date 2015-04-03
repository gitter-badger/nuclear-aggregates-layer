using System.Collections.Generic;
using System.Collections.ObjectModel;

using DoubleGis.Erm.Platform.Model.Entities.EAV;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    internal class EntityPartConverterHost<TEntity, TEntityInstance, TPropertyInstance> : IEntityPartConverterHost<TEntityInstance, TPropertyInstance>
        where TEntity : class, IEntity, IEntityKey, IAuditableEntity, IDeactivatableEntity, IDeletableEntity, IStateTrackingEntity, new()
        where TEntityInstance : class, IDynamicEntityInstance
        where TPropertyInstance : class, IDynamicEntityPropertyInstance
    {
        private readonly IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TPropertyInstance> _converter;

        public EntityPartConverterHost(IDynamicPropertiesConverterFactory converterFactory)
        {
            _converter = converterFactory.Create<TEntity, TEntityInstance, TPropertyInstance>();
        }

        public IEntity Convert(TEntityInstance entityInstance, ICollection<TPropertyInstance> propertyInstances)
        {
            return _converter.ConvertFromDynamicEntityInstance(entityInstance, propertyInstances);
        }

        public DynamicEntityInstanceDto<TEntityInstance, TPropertyInstance> ConvertBack(IEntity entity)
        {
            var entityPart = entity as IEntityPart;
            var propertyInstances = new Collection<TPropertyInstance>();
            var tuple = _converter.ConvertToDynamicEntityInstance((TEntity)entity, propertyInstances, entityPart != null ? entityPart.EntityId : (long?)null);
            return new DynamicEntityInstanceDto<TEntityInstance, TPropertyInstance>
                {
                    EntityInstance = tuple.Item1,
                    PropertyInstances = tuple.Item2
                };
        }
    }
}