using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22766, "Alters the activities schema to store the letters.", "s.pomadin")]
    public class Migration22766 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Resources.Migration_22766_AlterSchema);
            context.Connection.ExecuteNonQuery(Resources.Migration_22766_AddIndexes);
        }
    }
}
