using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public abstract class EFDomainContextFactory : IReadDomainContextFactory, IModifiableDomainContextFactory
    {
        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;
        private readonly IEFConnectionFactory _connectionFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IProducedQueryLogAccessor _producedQueryLogAccessor;
        private readonly ICommonLog _logger;
        private readonly IMsCrmReplicationMetadataProvider _msCrmReplicationMetadataProvider;

        protected EFDomainContextFactory(
            IEFConnectionFactory connectionFactory,
            IDomainContextMetadataProvider domainContextMetadataProvider,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            IProducedQueryLogAccessor producedQueryLogAccessor,
            ICommonLog logger,
            IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider)
        {
            _connectionFactory = connectionFactory;
            _domainContextMetadataProvider = domainContextMetadataProvider;
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
            var entityConnection = _connectionFactory.CreateEntityConnection(domainContextMetadata);
            var objectContext = new EFDbContext(entityConnection, _producedQueryLogAccessor);

            return new EFDomainContext(ProcessingContext,
                                       domainContextMetadata.EntityContainerName,
                                       objectContext,
                                       _pendingChangesHandlingStrategy,
                                       _msCrmReplicationMetadataProvider,
                                       _logger);
        }
    }
}
