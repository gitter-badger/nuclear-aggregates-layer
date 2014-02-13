using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(16533, "Создание индекса IX_OrderPositions_PricePositions_IsActive_IsDeleted в таблице OrderPositions", "d.ivanov")]
    public class Migration16533 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var orderPosition = ErmTableNames.OrderPositions;
            var table = context.Database.Tables[orderPosition.Name, orderPosition.Schema];
            table.CreateIndex("PricePositionId", "IsActive", "IsDeleted");
        }
    }
}