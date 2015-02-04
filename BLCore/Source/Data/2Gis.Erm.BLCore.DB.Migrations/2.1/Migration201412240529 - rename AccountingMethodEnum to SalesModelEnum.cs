using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412240529, "Переименовываем колонку AccountingMethodEnum в SalesModel", "y.baranihin")]
    public class Migration201412240529 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Positions);
            var columnToRename = table.Columns["AccountingMethodEnum"];
            if (columnToRename != null)
            {
                columnToRename.Rename("SalesModel");
            }
        }
    }
}
