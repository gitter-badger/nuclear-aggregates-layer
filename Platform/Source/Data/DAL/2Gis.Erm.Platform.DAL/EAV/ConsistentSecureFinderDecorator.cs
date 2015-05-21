﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public sealed class ConsistentSecureFinderDecorator : ISecureFinder
    {
        private readonly ISecureFinder _secureFinder;
        private readonly ICompositeEntityQuery _compositeEntityQuery;
        private readonly DynamicStorageFinderWrapper _dynamicStorageFinderWrapper;

        public ConsistentSecureFinderDecorator(ISecureFinder secureFinder,
                                               IDynamicStorageFinder dynamicStorageFinder,
                                               ICompositeEntityQuery compositeEntityQuery,
                                               IDynamicEntityMetadataProvider dynamicEntityMetadataProvider)
        {
            _secureFinder = secureFinder;
            _compositeEntityQuery = compositeEntityQuery;
            _dynamicStorageFinderWrapper = new DynamicStorageFinderWrapper(dynamicStorageFinder, dynamicEntityMetadataProvider);
        }

        public IQueryable<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return _secureFinder.Find(findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification,
                                                          FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return _secureFinder.Find(selectSpecification, findSpecification).ValidateQueryCorrectness();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
            return _secureFinder.Find(expression).ValidateQueryCorrectness();
        }

        public TEntity FindOne<TEntity>(FindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
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
                return _compositeEntityQuery.For(findSpecification).SingleOrDefault();
            }

            return Find(findSpecification).SingleOrDefault();
        }

        public TOutput FindOne<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification, FindSpecification<TEntity> findSpecification) 
            where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TEntity> FindMany<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
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
                return _compositeEntityQuery.For(findSpecification).ToArray();
            }

            return Find(findSpecification).ToArray();
        }

        public IReadOnlyCollection<TOutput> FindMany<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification, FindSpecification<TEntity> findSpecification) 
            where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public bool FindAny<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }
    }
}
