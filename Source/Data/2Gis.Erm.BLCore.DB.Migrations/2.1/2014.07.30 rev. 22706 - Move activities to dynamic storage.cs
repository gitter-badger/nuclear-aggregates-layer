using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22706, "Updates the activities schema and data.", "s.pomadin")]
    public class Migration22706 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Resources.Migration_22706_Alter_Activity_Schema);
            context.Connection.ExecuteNonQuery(Resources.Migration_22706_Migrate_Appointments);
            context.Connection.ExecuteNonQuery(Resources.Migration_22706_Migrate_Phonecalls);
            context.Connection.ExecuteNonQuery(Resources.Migration_22706_Migrate_Tasks);
            context.Connection.ExecuteNonQuery(Resources.Migration_22706_Drop_Old_Activity_Schema);
        }
    }
}
