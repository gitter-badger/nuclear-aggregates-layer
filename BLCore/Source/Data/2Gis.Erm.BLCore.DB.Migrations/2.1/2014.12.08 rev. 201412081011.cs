using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412081011, "[ERM-5408]: добавление процедур массовой репликации", "s.pomadin")]
    public class Migration201412081011 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Resources.Migration201412081011_Create_ReplicateAppointments);
            context.Connection.ExecuteNonQuery(Resources.Migration201412081011_Create_ReplicateLetters);
            context.Connection.ExecuteNonQuery(Resources.Migration201412081011_Create_ReplicatePhonecalls);
            context.Connection.ExecuteNonQuery(Resources.Migration201412081011_Create_ReplicateTasks);
        }
    }
}
