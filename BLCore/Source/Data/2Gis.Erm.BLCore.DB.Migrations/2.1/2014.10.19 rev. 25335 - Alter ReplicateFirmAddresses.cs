using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25335, "Не реплицируем связь адреса с удаленной рубрикой", "a.tukaev")]
    public class Migration25335 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(Resources._BusinessDirectory___ReplicateFirmAddresses_25335);
        }
    }
}