using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Qds.Migrations.Base;

namespace DoubleGis.Erm.Qds.Migrations
{
    [Migration(201503040120, "Replicate firms", "m.pashuk")]
    public class Migration201503040120 : ElasticSearchMigration
    {
        public override void Apply(IElasticSearchMigrationContext context)
        {
            context.ReplicationQueue.Add("FirmGridDoc");
        }
    }
}