using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    internal sealed class DynamicStorageFinderWrapper
    {
        private readonly IDynamicStorageFinder _dynamicStorageFinder;
        private readonly IDynamicEntityMetadataProvider _dynamicEntityMetadataProvider;

        public DynamicStorageFinderWrapper(IDynamicStorageFinder dynamicStorageFinder, IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            _dynamicStorageFinder = dynamicStorageFinder;
            _dynamicEntityMetadataProvider = dynamicEntityMetadataProvider;
        }

        public IEnumerable<TEntity> FindDynamic<TEntity>(Func<IQueryable, IQueryable> accessRestrictor, params long[] ids)
        {
            var specs = _dynamicEntityMetadataProvider.GetSpecifications<DictionaryEntityInstance, DictionaryEntityPropertyInstance>(typeof(TEntity), ids);
            return accessRestrictor(_dynamicStorageFinder.Find(specs).AsQueryable()).Cast<TEntity>();
        }

        public IReadOnlyCollection<TEntity> FindMany<TEntity>(Func<IFindSpecification<TEntity>, IQueryable<TEntity>> queryExecutor, IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var entities = queryExecutor(findSpecification).ToArray();
				var specs = _dynamicEntityMetadataProvider.GetSpecifications<BusinessEntityInstance, BusinessEntityPropertyInstance>(typeof(TEntity), entities.OfType<IEntityKey>().Select(e => e.Id));
                var parts = _dynamicStorageFinder.Find(specs).Cast<IEntityPart>().GroupBy(part => part.EntityId).ToDictionary(group => group.Key);

                foreach (IEntityKey entity in entities)
                {
                    IGrouping<long, IEntityPart> entityParts;
                    if (parts.TryGetValue(entity.Id, out entityParts))
                    {
                        ((IPartable)entity).Parts = entityParts.ToArray();
                    }
                }

                transaction.Complete();

                return entities;
            }
        }

        public TEntity FindOne<TEntity>(Func<IFindSpecification<TEntity>, IQueryable<TEntity>> queryExecutor, IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var entity = queryExecutor(findSpecification).SingleOrDefault();
                if (entity is IEntityKey)
                {
                    var specs = _dynamicEntityMetadataProvider.GetSpecifications<BusinessEntityInstance, BusinessEntityPropertyInstance>(entity.GetType(), new[] { ((IEntityKey)entity).Id });
                    ((IPartable)entity).Parts = _dynamicStorageFinder.Find(specs).Cast<IEntityPart>();
                }

                transaction.Complete();

                return entity;
            }
        }
    }
}
