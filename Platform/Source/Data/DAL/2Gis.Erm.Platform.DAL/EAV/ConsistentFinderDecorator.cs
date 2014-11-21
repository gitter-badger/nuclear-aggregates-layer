using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            IQueryable<TEntity> mappedQueryable;
            if (TryFindMapped(findSpecification, out mappedQueryable))
            {
                return mappedQueryable;
            }

            return _finder.Find(findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification,
                                                          IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
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

        public IQueryable FindAll(Type entityType)
        {
            return _finder.FindAll(entityType).ValidateQueryCorrectness();
        }

        public IQueryable<TEntity> FindAll<TEntity>() where TEntity : class, IEntity
        {
            return _finder.FindAll<TEntity>().ValidateQueryCorrectness();
        }

        public TEntity FindOne<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity, IEntityKey
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

        public IEnumerable<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TEntity)))
            {
                return _dynamicStorageFinderWrapper.FindMany(_finder.Find, findSpecification);
            }

            if (typeof(TEntity).AsEntityName().IsDynamic())
            {
                var ids = findSpecification.ExtractEntityIds();
                return _dynamicStorageFinderWrapper.FindDynamic<TEntity>(q => q, ids).AsEnumerable();
            }

            IQueryable<TEntity> mappedQueryable;
            if (TryFindMapped(findSpecification, out mappedQueryable))
            {
                return mappedQueryable.AsEnumerable();
            }

            return Find(findSpecification).AsEnumerable();
        }

        private bool TryFindMapped<TEntity>(IFindSpecification<TEntity> findSpecification, out IQueryable<TEntity> queryable)
        {
            var entityType = typeof(TEntity);

            EntityName entityName;
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