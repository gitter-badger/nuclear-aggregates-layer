using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(19684, "Alter UpdateBuildings (ERM-2904 fix)", "a.tukaev")]
    public class Migration19684 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.Migration19684);
        }
    }
}