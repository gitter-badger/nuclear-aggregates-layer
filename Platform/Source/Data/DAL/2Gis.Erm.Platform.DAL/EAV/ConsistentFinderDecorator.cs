using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class ConsistentFinderDecorator : IFinder
    {
        private readonly IFinder _finder;
        private readonly DynamicStorageFinderWrapper _dynamicStorageFinderWrapper;
        private readonly ICompositeEntityDecorator _compositeEntityDecorator;

        public ConsistentFinderDecorator(IFinder finder,
                                         IDynamicStorageFinder dynamicStorageFinder,
                                         ICompositeEntityDecorator compositeEntityDecorator,
                                         IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            _finder = finder;
            _compositeEntityDecorator = compositeEntityDecorator;
            _dynamicStorageFinderWrapper = new DynamicStorageFinderWrapper(dynamicStorageFinder, dynamicEntityMetadataProvider);
        }

        public IQueryable<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            IQueryable<TEntity> mappedQueryable;
            if (TryFindMapped(findSpecification, out mappedQueryable))
            {
                return mappedQueryable;
            }

            return _finder.Find(findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification,
                                                          FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            IQueryable<TEntity> mappedQueryable;
            if (TryFindMapped(findSpecification, out mappedQueryable))
            {
                return mappedQueryable.Select(selectSpecification.Selector);
            }

            return _finder.Find(selectSpecification, findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
            IQueryable<TEntity> mappedQueryable;
            if (TryFindMapped(new FindSpecification<TEntity>(expression), out mappedQueryable))
            {
                return mappedQueryable;
            }

            return _finder.Find(expression).ValidateQueryCorrectness();
        }

        public TEntity FindOne<TEntity>(FindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TEntity)))
            {
                return _dynamicStorageFinderWrapper.FindOne(_finder.Find, findSpecification);
            }

            if (typeof(TEntity).AsEntityName().IsDynamic())
            {
                var id = findSpecification.ExtractEntityId();
                return _dynamicStorageFinderWrapper.FindDynamic<TEntity>(q => q, id).SingleOrDefault();
            }

            IQueryable<TEntity> mappedQueryable;
            if (TryFindMapped(findSpecification, out mappedQueryable))
            {
                return mappedQueryable.SingleOrDefault();
            }

            return Find(findSpecification).SingleOrDefault();
        }

        public IEnumerable<TEntity> FindMany<TEntity>(FindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TEntity)))
            {
                return _dynamicStorageFinderWrapper.FindMany(_finder.Find, findSpecification);
            }

            if (typeof(TEntity).AsEntityName().IsDynamic())
            {
                var ids = findSpecification.ExtractEntityIds();
                return _dynamicStorageFinderWrapper.FindDynamic<TEntity>(q => q, ids).ToArray();
            }

            IQueryable<TEntity> mappedQueryable;
            if (TryFindMapped(findSpecification, out mappedQueryable))
            {
                return mappedQueryable.ToArray();
            }

            return Find(findSpecification).ToArray();
        }

        private bool TryFindMapped<TEntity>(FindSpecification<TEntity> findSpecification, out IQueryable<TEntity> queryable) where TEntity : class
        {
            var entityType = typeof(TEntity);

            IEntityType entityName;
            if (entityType.TryGetEntityName(out entityName) && entityName.HasMapping())
            {
                queryable = _compositeEntityDecorator.Find(findSpecification);
                return true;
            }

            queryable = null;
            return false;
        }
    }
}