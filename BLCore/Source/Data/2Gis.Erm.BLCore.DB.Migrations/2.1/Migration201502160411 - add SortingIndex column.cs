using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502160411, "Добавление колонки SortingIndex в таблицу [Billing].[Positions]", "a.rechkalov")]
    public class Migration201502160411 : TransactedMigration
    {
        private const string ColumnName = "SortingIndex";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Positions);
            if (table == null || table.Columns.Contains(ColumnName))
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(1, t => new Column(t, ColumnName, DataType.Int) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }
    }
}
