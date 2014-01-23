using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Qds.API.Operations.Indexers.Raw;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public interface IElasticSearchMigrationContext : IMigrationContextBase
    {
        IElasticClient ElasticClient { get; }
        IRawDocumentIndexer RawDocumentIndexer { get; }
        string GetIndexName(string indexName);
    }
}