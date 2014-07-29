using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(8756, "Изменения в некоторых хранимках репликации, т.к. ушел OwnerCode")]
    public sealed class Migration8756 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var command = Resources._2013_05_15_rev__8756___alter_some_replicate_sps;
            context.Connection.ExecuteNonQuery(command);
        }
    }
}
