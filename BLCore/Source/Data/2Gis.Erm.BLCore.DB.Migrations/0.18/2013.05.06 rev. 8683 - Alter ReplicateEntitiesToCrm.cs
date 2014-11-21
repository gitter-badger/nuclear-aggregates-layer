using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8683, "Изменения в хранимке ReplicateEntitiesToCrm")]
    public sealed class Migration8683 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var command = Resources._2013_05_06_rev__8683___ReplicateEntitiesToCrm;
            context.Connection.ExecuteNonQuery(command);
        }
    }
}
