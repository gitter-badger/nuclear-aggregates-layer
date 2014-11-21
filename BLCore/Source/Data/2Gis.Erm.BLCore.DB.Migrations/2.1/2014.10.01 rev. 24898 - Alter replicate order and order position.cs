using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(24898, "Повторное выпиливание зареспавнившихся лайков", "a.tukaev")]
    public class Migration24898 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrder_24898);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrderPosition_24898);
        }
    }
}