using System.Data.SqlClient;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFDomainContextFactory : IReadDomainContextFactory, IModifiableDomainContextFactory
    {
        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IEfDbModelFactory _efDbModelFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IProcessingContext _processingContext;
        private readonly IProducedQueryLogAccessor _producedQueryLogAccessor;
        private readonly ITracer _tracer;
        private readonly IMsCrmReplicationMetadataProvider _msCrmReplicationMetadataProvider;

        public EFDomainContextFactory(IDomainContextMetadataProvider domainContextMetadataProvider,
                                      IConnectionStringSettings connectionStringSettings,
                                      IEfDbModelFactory efDbModelFactory,
                                      IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                                      IProcessingContext processingContext,
                                      IProducedQueryLogAccessor producedQueryLogAccessor,
                                      ITracer tracer,
                                      IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider)
        {
            _domainContextMetadataProvider = domainContextMetadataProvider;
            _connectionStringSettings = connectionStringSettings;
            _efDbModelFactory = efDbModelFactory;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _processingContext = processingContext;
            _producedQueryLogAccessor = producedQueryLogAccessor;
            _tracer = tracer;
            _msCrmReplicationMetadataProvider = msCrmReplicationMetadataProvider;
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
            var dbContext = new EFDbContext(connection, model, _producedQueryLogAccessor, true);

            return new EFDomainContext(_processingContext,
                                       domainContextMetadata.EntityContainerName,
                                       dbContext,
                                       _pendingChangesHandlingStrategy,
                                       _msCrmReplicationMetadataProvider,
                                       _tracer);
        }
    }
}