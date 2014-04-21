using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Specs;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel
{
    // TODO {all, 07.04.2014}: опять в целом по EAV комментарий смешаны ответственности DAL, Mapper для parts, предлагается пока дальшене развивать этот подход, без предварительного анализа другихвариантов использования EAV   
    public static class EavFinderExtensions
    {
        public static TEntity GetEntityWithPart<TEntity, TEntityPart>(this IFinder finder,
                                                                FindSpecification<TEntity> findSpec,
                                                                IBusinessEntityPropertiesConverter<TEntityPart> propertiesConverter)
            where TEntityPart : class, IEntity, IEntityPart
            where TEntity : class, IEntity, IEntityKey, IPartable
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var entity = finder.Find(findSpec).Single();
                var part = finder.SingleOrDefault(entity.Id, propertiesConverter);
                entity.Parts = new[] { part };

                transactionScope.Complete();

                return entity;
            }
        }

        public static TEntityPart SingleOrDefault<TEntityPart>(this IFinder finder,
                                                               long referencedEntityId,
                                                               IBusinessEntityPropertiesConverter<TEntityPart> propertiesConverter)
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

            var entityPart = propertiesConverter.ConvertFromDynamicEntityInstance(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
            entityPart.EntityId = entityInstanceDto.EntityInstance.EntityId.Value;

            return entityPart;
        }

        public static BusinessEntityInstanceDto Single<TEntityPart>(this IFinder finder,
                                                                    TEntityPart entityPart,
                                                                    IBusinessEntityPropertiesConverter<TEntityPart> propertiesConverter)
            where TEntityPart : class, IEntity, IEntityPart
        {
            var dto = SingleOrDefault(finder, entityPart.EntityId);

            var propertyInstances = dto != null ? dto.PropertyInstances : new Collection<BusinessEntityPropertyInstance>();
            var tuple = propertiesConverter.ConvertToDynamicEntityInstance(entityPart, propertyInstances, entityPart.EntityId);

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