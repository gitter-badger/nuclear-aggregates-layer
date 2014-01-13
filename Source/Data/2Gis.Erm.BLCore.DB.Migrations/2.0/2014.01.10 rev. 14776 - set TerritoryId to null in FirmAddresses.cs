using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(14776, "Обнуляем значения TerritoryId в таблице FirmAddresses", "a.tukaev")]
    public class Migration14776 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery("UPDATE BusinessDirectory.FirmAddresses SET TerritoryId = NULL");
        }
    }
}