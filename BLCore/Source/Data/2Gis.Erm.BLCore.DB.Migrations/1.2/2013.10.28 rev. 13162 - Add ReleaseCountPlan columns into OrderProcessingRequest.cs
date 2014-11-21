using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13162, "Добавляем в таблицу OrderProcessingRequests поле ReleaseCountPlan")]
    public class Migration13162 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string ReleaseCountPlanColumnName = "ReleaseCountPlan";

            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(6, x => new Column(x, ReleaseCountPlanColumnName, DataType.Int) { Nullable = false })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }
    }
}
