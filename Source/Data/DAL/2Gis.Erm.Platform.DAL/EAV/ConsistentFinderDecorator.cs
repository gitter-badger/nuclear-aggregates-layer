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

        public ConsistentFinderDecorator(IFinder finder,
                                         IDynamicStorageFinder dynamicStorageFinder,
                                         IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            _finder = finder;
            _dynamicStorageFinderWrapper = new DynamicStorageFinderWrapper(dynamicStorageFinder, dynamicEntityMetadataProvider);
        }

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return _finder.Find(findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification,
                                                          IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return _finder.Find(selectSpecification, findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
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

            return Find(findSpecification).SingleOrDefault();
        }

        public IReadOnlyCollection<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity, IEntityKey
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

            return Find(findSpecification).ToArray();
        }
    }
}