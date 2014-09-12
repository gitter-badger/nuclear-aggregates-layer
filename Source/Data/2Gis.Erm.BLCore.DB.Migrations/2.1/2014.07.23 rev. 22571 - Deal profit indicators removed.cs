using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22571, "[ERM-4503] Удаление пересчета показателей сделки", "i.maslennikov")]
    public class Migration22571 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var dealsTable = context.Database.GetTable(ErmTableNames.Deals);
            var actualProfitColumn = dealsTable.Columns["ActualProfit"];
            var estimatedProfitColumn = dealsTable.Columns["EstimatedProfit"];
            if (actualProfitColumn != null && estimatedProfitColumn != null)
            {
                context.Database.ExecuteNonQuery(Resources._22571_ReplicateDeal);

                actualProfitColumn.Drop();
                estimatedProfitColumn.Drop();
            }
        }
    }
}
