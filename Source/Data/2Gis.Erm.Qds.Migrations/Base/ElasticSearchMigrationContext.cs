using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public sealed class ElasticSearchMigrationContext : IElasticSearchMigrationContext
    {
        public ElasticSearchMigrationContext(ReplicationQueueHelper replicationQueue, IElasticManagementApi managementApi, IElasticMetadataApi metadataApi)
        {
            ManagementApi = managementApi;
            ReplicationQueue = replicationQueue;
            MetadataApi = metadataApi;
        }

        public ReplicationQueueHelper ReplicationQueue { get; private set; }
        public IElasticManagementApi ManagementApi { get; private set; }
        public IElasticMetadataApi MetadataApi { get; private set; }
    }
}