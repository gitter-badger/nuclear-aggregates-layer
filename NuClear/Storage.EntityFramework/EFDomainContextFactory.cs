using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

using NuClear.Storage.ConnectionStrings;
using NuClear.Storage.Core;
using NuClear.Storage.UseCases;

namespace NuClear.Storage.EntityFramework
{
    public sealed class EFDomainContextFactory : IReadDomainContextFactory, IModifiableDomainContextFactory
    {
        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IEFDbModelFactory _efDbModelFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IProcessingContext _processingContext;
        private readonly IProducedQueryLogAccessor _producedQueryLogAccessor;

        public EFDomainContextFactory(IDomainContextMetadataProvider domainContextMetadataProvider,
                                      IConnectionStringSettings connectionStringSettings,
                                      IEFDbModelFactory efDbModelFactory,
                                      IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                                      IProcessingContext processingContext,
                                      IProducedQueryLogAccessor producedQueryLogAccessor)
        {
            _domainContextMetadataProvider = domainContextMetadataProvider;
            _connectionStringSettings = connectionStringSettings;
            _efDbModelFactory = efDbModelFactory;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _processingContext = processingContext;
            _producedQueryLogAccessor = producedQueryLogAccessor;
        }

        IReadDomainContext IReadDomainContextFactory.Create(DomainContextMetadata domainContextMetadata)
        {
            var readDomainContext = CreateDomainContext(domainContextMetadata);
            return readDomainContext;
        }

        IModifiableDomainContext IDomainContextFactory<IModifiableDomainContext>.Create<TEntity>()
        {
            var domainContextMetadata = _domainContextMetadataProvider.GetWriteMetadata(typeof(TEntity));

            var modifiableDomainContext = CreateDomainContext(domainContextMetadata);
            return modifiableDomainContext;
        }

        private EFDomainContext CreateDomainContext(DomainContextMetadata domainContextMetadata)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(domainContextMetadata.ConnectionStringName);
            var connection = new SqlConnection(connectionString);
            var model = _efDbModelFactory.Create(domainContextMetadata.EntityContainerName, connection);

            var dbContext = CreateDbContext(connection, model);

            return new EFDomainContext(_processingContext,
                                       dbContext,
                                       _pendingChangesHandlingStrategy);
        }

        private DbContext CreateDbContext(DbConnection connection, DbCompiledModel model)
        {
            var dbContext = new DbContext(connection, model, true);
            dbContext.Configuration.ValidateOnSaveEnabled = true;
            dbContext.Configuration.UseDatabaseNullSemantics = true;
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;
            dbContext.Configuration.AutoDetectChangesEnabled = false;
            dbContext.Database.Log = _producedQueryLogAccessor.Log;
            return dbContext;
        }
    }
}