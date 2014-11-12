using System.Data.SqlClient;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public abstract class EFDomainContextFactory : IReadDomainContextFactory, IModifiableDomainContextFactory
    {
        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IEfDbModelFactory _efDbModelFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IProducedQueryLogAccessor _producedQueryLogAccessor;
        private readonly ICommonLog _logger;
        private readonly IMsCrmReplicationMetadataProvider _msCrmReplicationMetadataProvider;

        protected EFDomainContextFactory(IDomainContextMetadataProvider domainContextMetadataProvider,
                                         IConnectionStringSettings connectionStringSettings,
                                         IEfDbModelFactory efDbModelFactory,
                                         IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                                         IProducedQueryLogAccessor producedQueryLogAccessor,
                                         ICommonLog logger,
                                         IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider)
        {
            _domainContextMetadataProvider = domainContextMetadataProvider;
            _connectionStringSettings = connectionStringSettings;
            _efDbModelFactory = efDbModelFactory;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _producedQueryLogAccessor = producedQueryLogAccessor;
            _logger = logger;
            _msCrmReplicationMetadataProvider = msCrmReplicationMetadataProvider;
        }

        protected abstract IProcessingContext ProcessingContext { get; }

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
            var dbContext = new EFDbContext(connection, model, _producedQueryLogAccessor);

            return new EFDomainContext(ProcessingContext,
                                       domainContextMetadata.EntityContainerName,
                                       dbContext,
                                       _pendingChangesHandlingStrategy,
                                       _msCrmReplicationMetadataProvider,
                                       _logger);
        }
    }
}
