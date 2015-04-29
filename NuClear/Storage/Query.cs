using System;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Core;

namespace NuClear.Storage
{
    public class Query : IQuery
    {
        private readonly IReadDomainContextProvider _readDomainContextProvider;

        public Query(IReadDomainContextProvider readDomainContextProvider)
        {
            _readDomainContextProvider = readDomainContextProvider;
        }

        public IQueryable For(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return _readDomainContextProvider.Get().GetQueryableSource(entityType);
        }

        public IQueryable<TEntity> For<TEntity>() where TEntity : class, IEntity
        {
            return _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
        }
    }
}