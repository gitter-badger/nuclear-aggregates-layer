using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412021150, "[ERM-5358]: fixed regardingobject reference resolving", "s.pomadin")]
    public class Migration201412021150 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.Migration201412021150_Alter_ReplicateAppointment);
            context.Database.ExecuteNonQuery(Resources.Migration201412021150_Alter_ReplicateLetter);
            context.Database.ExecuteNonQuery(Resources.Migration201412021150_Alter_ReplicatePhonecall);
            context.Database.ExecuteNonQuery(Resources.Migration201412021150_Alter_ReplicateTask);
        }
    }
}
