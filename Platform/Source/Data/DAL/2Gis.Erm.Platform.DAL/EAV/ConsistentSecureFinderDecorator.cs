using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class ConsistentSecureFinderDecorator : ISecureFinder
    {
        private readonly ISecureFinder _secureFinder;
        private readonly ICompositeEntityDecorator _compositeEntityDecorator;
        private readonly DynamicStorageFinderWrapper _dynamicStorageFinderWrapper;

        public ConsistentSecureFinderDecorator(ISecureFinder secureFinder,
                                               IDynamicStorageFinder dynamicStorageFinder,
                                               ICompositeEntityDecorator compositeEntityDecorator,
                                               IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            _secureFinder = secureFinder;
            _compositeEntityDecorator = compositeEntityDecorator;
            _dynamicStorageFinderWrapper = new DynamicStorageFinderWrapper(dynamicStorageFinder, dynamicEntityMetadataProvider);
        }

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return _secureFinder.Find(findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification,
                                                          IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return _secureFinder.Find(selectSpecification, findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
            return _secureFinder.Find(expression).ValidateQueryCorrectness();
        }

        public IQueryable For(Type entityType)
        {
            return _secureFinder.For(entityType).ValidateQueryCorrectness();
        }

        public IQueryable<TEntity> For<TEntity>() where TEntity : class, IEntity
        {
            return _secureFinder.For<TEntity>().ValidateQueryCorrectness();
        }

        public TEntity FindOne<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity, IEntityKey
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TEntity)))
            {
                return _dynamicStorageFinderWrapper.FindOne(_secureFinder.Find, findSpecification);
            }

            if (typeof(TEntity).AsEntityName().IsDynamic())
            {
                var id = findSpecification.ExtractEntityId();
                // FIXME {d.ivanov, 06.05.2014}: Заменить q => q на реальный restrictor
                return _dynamicStorageFinderWrapper.FindDynamic<TEntity>(q => q, id).SingleOrDefault();
            }

            if (typeof(TEntity).AsEntityName().HasMapping())
            {
                return _compositeEntityDecorator.Find(findSpecification).SingleOrDefault();
            }

            return Find(findSpecification).SingleOrDefault();
        }

        public IEnumerable<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity, IEntityKey
        {
            if (typeof(IPartable).IsAssignableFrom(typeof(TEntity)))
            {
                return _dynamicStorageFinderWrapper.FindMany(_secureFinder.Find, findSpecification);
            }

            if (typeof(TEntity).AsEntityName().IsDynamic())
            {
                var ids = findSpecification.ExtractEntityIds();
                // FIXME {d.ivanov, 06.05.2014}: Заменить q => q на реальный restrictor
                return _dynamicStorageFinderWrapper.FindDynamic<TEntity>(q => q, ids).ToArray();
            }

            if (typeof(TEntity).AsEntityName().HasMapping())
            {
                return _compositeEntityDecorator.Find(findSpecification).AsEnumerable();
            }

            return Find(findSpecification).ToArray();
        }
    }
}
