using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(24481, "Creates the procedures to replicate the activities.", "s.pomadin")]
    public class Migration24481 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Resources.Migration_24481_ReplicateAppointment);
            context.Connection.ExecuteNonQuery(Resources.Migration_24481_ReplicatePhonecall);
            context.Connection.ExecuteNonQuery(Resources.Migration_24481_ReplicateTask);
            context.Connection.ExecuteNonQuery(Resources.Migration_24481_ReplicateLetter);
        }
    }
}
