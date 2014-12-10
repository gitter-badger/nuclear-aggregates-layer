using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201411261501, "Fix batch replication procedures", "a.tukaev")]
    public class Migration201411261501 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources.ReplicateDeals_201411261501);
            context.Database.ExecuteNonQuery(Resources.ReplicateLegalPersons_201411261501);
            context.Database.ExecuteNonQuery(Resources.ReplicateLimits_201411261501);
            context.Database.ExecuteNonQuery(Resources.ReplicateOrders_201411261501); 
        }
    }
}