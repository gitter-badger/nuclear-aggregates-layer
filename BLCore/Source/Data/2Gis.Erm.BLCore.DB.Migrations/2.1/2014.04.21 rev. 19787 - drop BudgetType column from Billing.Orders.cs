using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(19787, "Удаление Orders.BudgetType", "a.tukaev")]
    public class Migration19787 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var orders = context.Database.GetTable(ErmTableNames.Orders);
            var budgetType = orders.Columns["BudgetType"];
            if (budgetType != null)
            {
                budgetType.Drop();
            }
        }
    }
}