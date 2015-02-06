using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502061213, "ERM-5657. Убрать цель в действии типа Встреча", "a.pashkin")]
    public class Migration201502061213 : TransactedMigration
    {
        private const string UpdateQuery = "UPDATE [Activity].[AppointmentBase] SET Purpose = 3 WHERE Purpose = 1";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(UpdateQuery);
        }
    }
}
