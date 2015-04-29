using System;

using NuClear.Storage.ConnectionStrings;

namespace NuClear.Storage.Core
{
    public sealed class DomainContextMetadataProvider : IDomainContextMetadataProvider
    {
        private readonly IEntityContainerNameResolver _entityContainerNameResolver;
        private readonly IConnectionStringNameResolver _connectionStringNameResolver;

        public DomainContextMetadataProvider(IEntityContainerNameResolver entityContainerNameResolver, IConnectionStringNameResolver connectionStringNameResolver)
        {
            _entityContainerNameResolver = entityContainerNameResolver;
            _connectionStringNameResolver = connectionStringNameResolver;
        }
         
        public DomainContextMetadata GetReadMetadata(Type entityType)
        {
            return GetMetadata(entityType, _connectionStringNameResolver.ResolveRead);
        }

        public DomainContextMetadata GetWriteMetadata(Type entityType)
        {
            return GetMetadata(entityType, _connectionStringNameResolver.ResolveWrite);
        }

        private DomainContextMetadata GetMetadata(Type entityType, Func<string, IConnectionStringIdentity> getConnectionStringNameFunc)
        {
            var entityContainerName = _entityContainerNameResolver.Resolve(entityType);
            return new DomainContextMetadata { EntityContainerName = entityContainerName, ConnectionStringName = getConnectionStringNameFunc(entityContainerName) };
        }
    }
}