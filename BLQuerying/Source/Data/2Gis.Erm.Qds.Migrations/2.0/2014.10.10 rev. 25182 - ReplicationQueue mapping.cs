using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(25182, "ReplicationQueue mapping", "m.pashuk")]
    public sealed class Migration25182 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            PutReplicationQueueMapping(context);
        }

        private static void PutReplicationQueueMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<ReplicationQueue25182>("Metadata", "ReplicationQueue");

            context.ManagementApi.Map<ReplicationQueue25182>(m => m

                .Properties(p => p
                    .String(s => s.Name(n => n.Progress).Index(FieldIndexOption.No))
                )
            );
        }

        public sealed class ReplicationQueue25182
        {
            public string DocumentType { get; set; }
            public ICollection<IndexSettings> IndexesSettings { get; set; }
            public string Progress { get; set; }

            public sealed class IndexSettings
            {
                public string IndexName { get; set; }
                public int? NumberOfReplicas { get; set; }
                public string RefreshInterval { get; set; }
            }
        }
    }
}