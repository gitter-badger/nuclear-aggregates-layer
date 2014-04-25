using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public abstract class EFDomainContextFactory : IReadDomainContextFactory, IModifiableDomainContextFactory
    {
        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;
        private readonly IEFConnectionFactory _connectionFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly ICommonLog _logger;

        protected EFDomainContextFactory(
            IEFConnectionFactory connectionFactory,
            IDomainContextMetadataProvider domainContextMetadataProvider,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            IMsCrmSettings msCrmSettings,
            ICommonLog logger)
        {
            _connectionFactory = connectionFactory;
            _domainContextMetadataProvider = domainContextMetadataProvider;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _msCrmSettings = msCrmSettings;
            _logger = logger;
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
            var objectContext = new EFDbContext(entityConnection);

            var domainContext = new EFDomainContext(ProcessingContext, domainContextMetadata.EntityContainerName, objectContext, _pendingChangesHandlingStrategy, _msCrmSettings, _logger);
            return domainContext;
        }
    }
}
