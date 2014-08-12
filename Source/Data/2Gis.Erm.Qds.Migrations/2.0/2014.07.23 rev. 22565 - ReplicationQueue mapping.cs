using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(22565, "ReplicationQueue mapping", "m.pashuk")]
    public sealed class Migration22565 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            PutReplicationQueueMapping(context);
        }

        private static void PutReplicationQueueMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<ReplicationQueue22565>("Metadata", "ReplicationQueue");

            context.ManagementApi.Map<ReplicationQueue22565>(m => m

                .Properties(p => p
                    .Object<ReplicationQueue22565.IndexSettings>(o => o
                        .Name(n => n.IndexesSettings.First())
                        .Properties(pp => pp
                            .String(s => s.Name(n => n.IndexName).Index(FieldIndexOption.No))
                            .Number(s => s.Name(n => n.NumberOfReplicas).Index(NonStringIndexOption.No))
                            .String(s => s.Name(n => n.RefreshInterval).Index(FieldIndexOption.No))
                        )
                    )
                )
            );
        }

        private sealed class ReplicationQueue22565
        {
            public string DocumentType { get; set; }
            public ICollection<IndexSettings> IndexesSettings { get; set; }

            public sealed class IndexSettings
            {
                public string IndexName { get; set; }
                public int? NumberOfReplicas { get; set; }
                public string RefreshInterval { get; set; }
            }
        }
    }
}