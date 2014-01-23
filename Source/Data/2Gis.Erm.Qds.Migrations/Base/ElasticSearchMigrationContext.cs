using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;
using DoubleGis.Erm.Qds.Common.ElasticClient;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public sealed class ElasticSearchMigrationContext : IElasticSearchMigrationContext
    {
        private readonly IElasticConnectionSettingsFactory _elasticConnectionSettingsFactory;

        public ElasticSearchMigrationContext(IElasticClient elasticClient, IElasticConnectionSettingsFactory elasticConnectionSettingsFactory, IRawDocumentIndexer rawDocumentIndexer)
        {
            _elasticConnectionSettingsFactory = elasticConnectionSettingsFactory;
            ElasticClient = elasticClient;
            RawDocumentIndexer = rawDocumentIndexer;
        }

        public IElasticClient ElasticClient { get; private set; }
        public IRawDocumentIndexer RawDocumentIndexer { get; private set; }

        public string GetIndexName(string indexName)
        {
            return _elasticConnectionSettingsFactory.GetIsolatedIndexName(indexName);
        }
    }
}