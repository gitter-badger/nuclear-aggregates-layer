using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Migrations.Base
{
    public interface IElasticSearchMigrationContext : IMigrationContextBase
    {
        ReplicationQueueHelper ReplicationQueue { get; }
        IElasticManagementApi ManagementApi { get; }
        IElasticMetadataApi MetadataApi { get; }
    }
}