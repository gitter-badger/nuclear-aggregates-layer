using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL
{
    // возвращает один и тот же ReadDomainContext для всех типов сущностей (он реально нужен только один для всех read-операций)
    public sealed class ReadDomainContextCachingProxy : IReadDomainContext
    {
        private readonly IDictionary<string, IReadDomainContext> _readDomainContexts = new Dictionary<string, IReadDomainContext>();

        private readonly IReadDomainContextFactory _readDomainContextFactory;
        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;

        public ReadDomainContextCachingProxy(
            IReadDomainContextFactory readDomainContextFactory,
            IDomainContextMetadataProvider domainContextMetadataProvider)
        {
            _readDomainContextFactory = readDomainContextFactory;
            _domainContextMetadataProvider = domainContextMetadataProvider;
        }

        public IQueryable GetQueryableSource(Type entityType)
        {
            var domainContextMetadata = _domainContextMetadataProvider.GetReadMetadata(entityType);

            var queryableSource = GetReadDomainContextFromCache(domainContextMetadata).GetQueryableSource(entityType);
            return queryableSource;
        }

        public IQueryable<TEntity> GetQueryableSource<TEntity>() where TEntity : class
        {
            var domainContextMetadata = _domainContextMetadataProvider.GetReadMetadata(typeof(TEntity));

            var queryableSource = GetReadDomainContextFromCache(domainContextMetadata).GetQueryableSource<TEntity>();
            return queryableSource;
        }

        public void Dispose()
        {
            foreach (var readDomainContext in _readDomainContexts.Values)
            {
                readDomainContext.Dispose();
            }
        }

        private IReadDomainContext GetReadDomainContextFromCache(DomainContextMetadata domainContextMetadata)
        {
            IReadDomainContext context;
            if (_readDomainContexts.TryGetValue(domainContextMetadata.EntityContainerName, out context))
            {
                return context;
            }

            context = _readDomainContextFactory.Create(domainContextMetadata);
            _readDomainContexts.Add(domainContextMetadata.EntityContainerName, context);

            return context;
        }
    }
}