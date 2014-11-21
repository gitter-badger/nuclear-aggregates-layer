using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(24551, "ERM-4967 fix", "a.tukaev")]
    public class Migration24551 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateTerritory_24551);
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateTerritories_24551);
        }
    }
}