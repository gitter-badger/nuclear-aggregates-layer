using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class DynamicEntityMetadataProvider : IDynamicEntityMetadataProvider
    {
        public Type DetermineObjectType<TPropertyInstace>(IEnumerable<TPropertyInstace> dynamicEntityPropertyInstances)
            where TPropertyInstace : class, IDynamicEntityPropertyInstance
        {
            var entityTypeId = dynamicEntityPropertyInstances
                .Where(property => property.PropertyId == EntityTypeNameIdentity.Instance.Id)
                .Select(instance => (int?)instance.NumericValue)
                .SingleOrDefault();

            if (!entityTypeId.HasValue)
            {
                throw new InvalidOperationException("Entity part type cannot be determined using it's property instanses collection");
            }

            IEntityType entityType;
            if (!EntityType.Instance.TryParse(entityTypeId.Value, out entityType))
            {
                throw new InvalidOperationException(string.Format("Value '{0} <-> {1}' isn't definded within EntityName enum", entityTypeId.Value.ToString("F"), entityTypeId));
            }

            return entityType.AsEntityType();
        }

        public SpecsBundle<TEntityInstance, TEntityPropertyInstance> GetSpecifications<TEntityInstance, TEntityPropertyInstance>(Type entityType, IEnumerable<long> entityIds) 
            where TEntityInstance : class, IDynamicEntityInstance
            where TEntityPropertyInstance : class, IDynamicEntityPropertyInstance
        {
            IFindSpecification<TEntityInstance> findSpecification;
            ISelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>> selectSpecification;

            if (typeof(TEntityInstance) == typeof(BusinessEntityInstance) && typeof(TEntityPropertyInstance) == typeof(BusinessEntityPropertyInstance))
            {
                findSpecification = (IFindSpecification<TEntityInstance>)BusinessEntitySpecs.BusinessEntity.Find.ByReferencedEntities(entityIds);
                selectSpecification = (ISelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>>)BusinessEntitySpecs.BusinessEntity.Select.DynamicEntityInstanceDto();
                return new SpecsBundle<TEntityInstance, TEntityPropertyInstance>(findSpecification, selectSpecification);
            }

            if (typeof(TEntityInstance) == typeof(DictionaryEntityInstance) && typeof(TEntityPropertyInstance) == typeof(DictionaryEntityPropertyInstance))
            {
                findSpecification = (IFindSpecification<TEntityInstance>)Specs.Find.ByIds<DictionaryEntityInstance>(entityIds);
                selectSpecification = (ISelectSpecification<TEntityInstance, DynamicEntityInstanceDto<TEntityInstance, TEntityPropertyInstance>>)DictionaryEntitySpecs.DictionaryEntity.Select.DynamicEntityInstanceDto();
                return new SpecsBundle<TEntityInstance, TEntityPropertyInstance>(findSpecification, selectSpecification);
            }

            throw new InvalidOperationException("��� �����? " + typeof(TEntityInstance).Name);
        }
    }
}