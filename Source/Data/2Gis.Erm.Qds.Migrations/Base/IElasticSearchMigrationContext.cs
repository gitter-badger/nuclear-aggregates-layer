using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public interface IElasticSearchMigrationContext : IMigrationContextBase
    {
        IElasticManagementApi ElasticManagementApi { get; }
        ReplicationQueueHelper ReplicationQueue { get; }
        INestSettings NestSettings { get; }
    }
}