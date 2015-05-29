using System;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Core;
using NuClear.Storage.Specifications;

namespace NuClear.Storage
{
    public class Query : IQuery
    {
        private readonly IReadableDomainContextProvider _readableDomainContextProvider;

        public Query(IReadableDomainContextProvider readableDomainContextProvider)
        {
            _readableDomainContextProvider = readableDomainContextProvider;
        }

        public IQueryable For(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return _readableDomainContextProvider.Get().GetQueryableSource(entityType);
        }

        public IQueryable<TEntity> For<TEntity>() where TEntity : class, IEntity
        {
            return _readableDomainContextProvider.Get().GetQueryableSource<TEntity>();
        }

        public IQueryable<TEntity> For<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readableDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(findSpecification.Predicate);
        }
    }
}