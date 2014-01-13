using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10062, "Добавлена колонка [CategoryRate] в [Billing].[OrderPositions]")]
    public class Migration10062 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.OrderPositions);

            if (table.Columns.Contains("CategoryRate"))
            {
                return;
            }

            var columnsToInsert = new[]
                {
                    new InsertedColumnDefinition(5, x =>
                        {
                            var column = new Column(x, "CategoryRate", DataType.Decimal(4, 19)) { Nullable = false };
                            column.AddDefaultConstraint().Text = "0";
                            return column;
                        })
                };
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }
    }
}
