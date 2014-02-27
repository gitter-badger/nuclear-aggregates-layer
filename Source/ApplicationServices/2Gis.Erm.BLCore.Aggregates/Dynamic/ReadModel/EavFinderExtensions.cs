using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Specs;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel
{
    public static class EavFinderExtensions
    {
        public static TEntityPart SingleOrDefault<TEntityPart>(this IFinder finder,
                                                               long referencedEntityId,
                                                               Func<BusinessEntityInstance, ICollection<BusinessEntityPropertyInstance>, TEntityPart> propertiesConverter)
            where TEntityPart : class, IEntity, IEntityPart
        {
            var entityInstanceDto = SingleOrDefault(finder, referencedEntityId);
            if (entityInstanceDto == null)
            {
                return null;
            }

            if (!entityInstanceDto.EntityInstance.EntityId.HasValue)
            {
                throw new InvalidDataException("Partable entity has incorrect storage representation. Main entity reference is null");
            }

            var entityPart = propertiesConverter(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
            entityPart.EntityId = entityInstanceDto.EntityInstance.EntityId.Value;

            return entityPart;
        }

        public static BusinessEntityInstanceDto Single<TEntityPart>(
            this IFinder finder,
            TEntityPart entityPart,
            Func<TEntityPart, ICollection<BusinessEntityPropertyInstance>, long?, Tuple<BusinessEntityInstance, ICollection<BusinessEntityPropertyInstance>>> propertiesConverter)
            where TEntityPart : class, IEntity, IEntityPart
        {
            var dto = SingleOrDefault(finder, entityPart.EntityId);

            var propertyInstances = dto != null ? dto.PropertyInstances : new Collection<BusinessEntityPropertyInstance>();
            var tuple = propertiesConverter(entityPart, propertyInstances, entityPart.EntityId);

            return new BusinessEntityInstanceDto
                {
                    EntityInstance = tuple.Item1,
                    PropertyInstances = tuple.Item2
                };
        }
        
        private static BusinessEntityInstanceDto SingleOrDefault(this IFinder finder, long referencedEntityId)
        {
            return finder.Find<BusinessEntityInstance, BusinessEntityInstanceDto>(BusinessEntitySpecs.BusinessEntity.Select.BusinessEntityInstanceDto(),
                                                                                  BusinessEntitySpecs.BusinessEntity.Find.ByReferencedEntity(referencedEntityId))
                         .SingleOrDefault();
        }
    }
}