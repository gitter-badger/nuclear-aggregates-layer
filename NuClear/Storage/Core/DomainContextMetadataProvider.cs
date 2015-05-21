using System;

using NuClear.Storage.ConnectionStrings;

namespace NuClear.Storage.Core
{
    public sealed class DomainContextMetadataProvider : IDomainContextMetadataProvider
    {
        private readonly IEntityContainerNameResolver _entityContainerNameResolver;
        private readonly IConnectionStringIdentityResolver _connectionStringIdentityResolver;

        public DomainContextMetadataProvider(IEntityContainerNameResolver entityContainerNameResolver, IConnectionStringIdentityResolver connectionStringIdentityResolver)
        {
            _entityContainerNameResolver = entityContainerNameResolver;
            _connectionStringIdentityResolver = connectionStringIdentityResolver;
        }
         
        public DomainContextMetadata GetReadMetadata(Type entityType)
        {
            return GetMetadata(entityType, _connectionStringIdentityResolver.ResolveRead);
        }

        public DomainContextMetadata GetWriteMetadata(Type entityType)
        {
            return GetMetadata(entityType, _connectionStringIdentityResolver.ResolveWrite);
        }

        private DomainContextMetadata GetMetadata(Type entityType, Func<string, IConnectionStringIdentity> getConnectionStringNameFunc)
        {
            var entityContainerName = _entityContainerNameResolver.Resolve(entityType);
            return new DomainContextMetadata { EntityContainerName = entityContainerName, ConnectionStringName = getConnectionStringNameFunc(entityContainerName) };
        }
    }
}