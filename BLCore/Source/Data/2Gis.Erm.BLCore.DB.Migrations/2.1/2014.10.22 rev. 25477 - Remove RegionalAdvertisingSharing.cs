using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25477, "Удалим RegionalAdvertisingSharing", "y.baranihin")]
    public class Migration25477 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var orderRegionalAdvertisingSharingsTable = context.Database.GetTable(ErmTableNames.OrdersRegionalAdvertisingSharings);
            if (orderRegionalAdvertisingSharingsTable != null)
            {
                orderRegionalAdvertisingSharingsTable.Drop();
            }

            var regionalAdvertisingSharingsTable = context.Database.GetTable(ErmTableNames.RegionalAdvertisingSharings);
            if (regionalAdvertisingSharingsTable != null)
            {
                regionalAdvertisingSharingsTable.Drop();
            }
        }
    }
}