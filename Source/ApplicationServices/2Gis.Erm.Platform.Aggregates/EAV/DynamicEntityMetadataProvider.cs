using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    public sealed class DynamicEntityMetadataProvider : IDynamicEntityMetadataProvider
    {
        public Type DetermineObjectType<TPropertyInstace>(IEnumerable<TPropertyInstace> dynamicEntityPropertyInstances)
            where TPropertyInstace : class, IDynamicEntityPropertyInstance
        {
            var entityName = dynamicEntityPropertyInstances
                .Where(property => property.PropertyId == EntityTypeNameIdentity.Instance.Id)
                .Select(instance => (EntityName?)instance.NumericValue)
                .SingleOrDefault();

            if (!entityName.HasValue)
            {
                throw new InvalidOperationException("Entity part type cannot be determined using it's property instanses collection");
            }

            if (!Enum.IsDefined(typeof(EntityName), entityName))
            {
                throw new InvalidOperationException(string.Format("Value '{0} <-> {1}' isn't definded within EntityName enum", entityName.Value.ToString("F"), entityName));
            }

            return entityName.Value.AsEntityType();
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

            if (typeof(TEntityInstance) == typeof(ActivityInstance))
            {
                throw new NotImplementedException("Пока не сделано, но очень хотелось бы");
            }

            throw new InvalidOperationException("Кто здесь? " + typeof(TEntityInstance).Name);
        }
    }
}