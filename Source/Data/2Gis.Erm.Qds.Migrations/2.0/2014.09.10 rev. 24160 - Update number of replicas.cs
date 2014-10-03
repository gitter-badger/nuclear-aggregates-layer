using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Migrations.Base;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(24160, "Update number of replicas", "m.pashuk")]
    public sealed class Migration24160 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            context.MetadataApi.RegisterType<Data>("Data");
            context.MetadataApi.RegisterType<Metadata>("Metadata");

            context.ManagementApi.UpdateIndexSettings(typeof(Data), x => x.NumberOfReplicas(1));
            context.ManagementApi.UpdateIndexSettings(typeof(Metadata), x => x.NumberOfReplicas(1));
        }

        private sealed class Data { }
        private sealed class Metadata { }
    }
}