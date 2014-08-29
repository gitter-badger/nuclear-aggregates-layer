using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23630, "Creates the procedures to replicate the activities.", "s.pomadin")]
    public class Migration23630 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Resources.Migration_23630_GetCrmUserId);
            context.Connection.ExecuteNonQuery(Resources.Migration_23630_ReplicateAppointment);
            context.Connection.ExecuteNonQuery(Resources.Migration_23630_ReplicatePhonecall);
            context.Connection.ExecuteNonQuery(Resources.Migration_23630_ReplicateTask);
            context.Connection.ExecuteNonQuery(Resources.Migration_23630_ReplicateLetter);
        }
    }
}
