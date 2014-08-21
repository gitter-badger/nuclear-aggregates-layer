using System;

using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Migrations.Base;

using Nest;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(13546, "Создание поисковой инфраструктуры", "m.pashuk")]
    public sealed class Migration13546 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<MigrationDoc13546>("Metadata.13546", "MigrationDoc");
            context.ManagementApi.CreateIndex<MigrationDoc13546>(GetMetadataIndexDescriptor());
            context.ManagementApi.AddAlias<MigrationDoc13546>("Metadata");

            PutMigrationsMapping(context);
            PutReplicationQueueMapping(context);

            PutRecordIdStateMapping(context);
        }

        private static void PutReplicationQueueMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<ReplicationQueue13546>("Metadata.13546", "ReplicationQueue");

            context.ManagementApi.Map<ReplicationQueue13546>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.DocumentType).Index(FieldIndexOption.No))
                        )
            );
        }

        private void PutRecordIdStateMapping(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<RecordIdState13546>("Metadata.13546", "RecordIdState");

            context.ManagementApi.Map<RecordIdState13546>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))

                .Properties(p => p
                    .String(s => s.Name(n => n.Id).Index(FieldIndexOption.NotAnalyzed))
                    .String(s => s.Name(n => n.RecordId).Index(FieldIndexOption.No))
                        )
            );
        }

        private static void PutMigrationsMapping(IElasticSearchMigrationContext context)
        {
            context.ManagementApi.Map<MigrationDoc13546>(m => m
                .Dynamic(DynamicMappingOption.Strict)
                .DateDetection(false)
                .NumericDetection(false)
                .AllField(a => a.Enabled(false))
                .SourceField(s => s.Enabled(false))
            );
        }

        public static Func<CreateIndexDescriptor, CreateIndexDescriptor> GetMetadataIndexDescriptor()
        {
            return x => x
            .NumberOfShards(1)
            .NumberOfReplicas(2)
            .Settings(s => s.Add("refresh_interval", "1s"));
        }

        private sealed class ReplicationQueue13546
        {
            public string DocumentType { get; set; }
        }

        private sealed class MigrationDoc13546
        {
            public string Id { get; set; }
        }

        private sealed class RecordIdState13546
        {
            public string Id { get; set; }
            public string RecordId { get; private set; }
        }
    }
}