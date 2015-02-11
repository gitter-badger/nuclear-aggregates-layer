using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22776, "Alters activity schema to drop the redundant columns.", "s.pomadin")]
    public class Migration22776 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Resources.Migration_22776_AlterSchema);
        }
    }
}
