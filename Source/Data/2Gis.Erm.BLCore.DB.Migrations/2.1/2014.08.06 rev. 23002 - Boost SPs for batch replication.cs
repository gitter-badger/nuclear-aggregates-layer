using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23002, "Обновление хранимых процедур для массовой репликации, с целью ускорения выполнения", "i.maslennikov")]
    public class Migration23002 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // alter SPs
            context.Database.ExecuteNonQuery(Resources._23002_BusinessDirectory___ReplicateTerritories_);
            context.Database.ExecuteNonQuery(Resources._23002_BusinessDirectory___ReplicateFirms_);
            context.Database.ExecuteNonQuery(Resources._23002_BusinessDirectory___ReplicateFirmAddresses_);
        }
    }
}
