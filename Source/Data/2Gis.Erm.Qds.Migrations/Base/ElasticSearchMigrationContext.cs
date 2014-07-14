using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public sealed class ElasticSearchMigrationContext : IElasticSearchMigrationContext
    {
        public ElasticSearchMigrationContext(IElasticManagementApi elasticManagementApi, ReplicationQueueHelper replicationQueue, INestSettings nestSettings)
        {
            ElasticManagementApi = elasticManagementApi;
            ReplicationQueue = replicationQueue;
            NestSettings = nestSettings;
        }

        public IElasticManagementApi ElasticManagementApi { get; private set; }
        public ReplicationQueueHelper ReplicationQueue { get; private set; }
        public INestSettings NestSettings { get; private set; }
    }
}