using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(24442, "Массовые хранимые процедуры репликации", "a.tukaev")]
    public class Migration24442 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateAccountDetails_24442);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateAccounts_24442);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateLimits_24442);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateOrders_24442);
            context.Database.ExecuteNonQuery(Resources._Billing___ReplicateDeals_24442);
        }
    }
}