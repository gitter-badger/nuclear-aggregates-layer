using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Migrations.Base;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(22156, "Remove RecordIdState mapping", "m.pashuk")]
    public sealed class Migration22156 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            RemoveRecordIdStateMapping(context);
        }

        private static void RemoveRecordIdStateMapping(IElasticSearchMigrationContext context)
        {
            context.NestSettings.RegisterType<RecordIdState22156>("Metadata", "RecordIdState");
            context.ElasticManagementApi.DeleteMapping<RecordIdState22156>();
        }

        private sealed class RecordIdState22156 { }
    }
}