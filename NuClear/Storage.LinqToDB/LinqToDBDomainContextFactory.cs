using System.Transactions;

using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.Storage.ConnectionStrings;
using NuClear.Storage.Core;

namespace NuClear.Storage.LinqToDB
{
    public class LinqToDBDomainContextFactory : IReadableDomainContextFactory, IModifiableDomainContextFactory
    {
        private const int DefaultTimeout = 60;

        private readonly IDomainContextMetadataProvider _domainContextMetadataProvider;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly MappingSchema _mappingSchema;
        private readonly TransactionOptions _transactionOptions;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        public LinqToDBDomainContextFactory(
            IDomainContextMetadataProvider domainContextMetadataProvider,
            IConnectionStringSettings connectionStringSettings,
            MappingSchema mappingSchema,
            TransactionOptions transactionOptions,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
        {
            _domainContextMetadataProvider = domainContextMetadataProvider;
            _connectionStringSettings = connectionStringSettings;
            _mappingSchema = mappingSchema;
            _transactionOptions = transactionOptions;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
        }

        IReadableDomainContext IReadableDomainContextFactory.Create(DomainContextMetadata domainContextMetadata)
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

        private LinqToDBDomainContext CreateDomainContext(DomainContextMetadata domainContextMetadata)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(domainContextMetadata.ConnectionStringName);
            var dataConnection = CreateDataConnection(connectionString).AddMappingSchema(_mappingSchema);

            return new LinqToDBDomainContext(dataConnection, _transactionOptions, _pendingChangesHandlingStrategy);
        }

        private DataConnection CreateDataConnection(string connectionString)
        {
            var timeoutInSeconds = (int)_transactionOptions.Timeout.TotalSeconds;
            if (timeoutInSeconds == 0)
            {
                timeoutInSeconds = DefaultTimeout;
            }

            return new DataConnection(connectionString)
                {
                    CommandTimeout = timeoutInSeconds,
                    IsMarsEnabled = false
                };
        }
    }
}