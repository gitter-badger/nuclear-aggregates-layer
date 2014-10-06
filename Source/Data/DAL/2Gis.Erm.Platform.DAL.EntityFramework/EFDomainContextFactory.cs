using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public abstract class EFDomainContextFactory : IReadDomainContextFactory, IModifiableDomainContextFactory
    {
        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;
        private readonly IEFObjectContextFactory _connectionFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IProducedQueryLogAccessor _producedQueryLogAccessor;
        private readonly ICommonLog _logger;
        private readonly IMsCrmReplicationMetadataProvider _msCrmReplicationMetadataProvider;

        protected EFDomainContextFactory(
            IEFObjectContextFactory connectionFactory,
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
            var objectContext = _connectionFactory.CreateObjectContext(domainContextMetadata);
            var dbContext = new EFDbContext(objectContext, _producedQueryLogAccessor);

            return new EFDomainContext(ProcessingContext,
                                       domainContextMetadata.EntityContainerName,
                                       dbContext,
                                       _pendingChangesHandlingStrategy,
                                       _msCrmReplicationMetadataProvider,
                                       _logger);
        }
    }
}
