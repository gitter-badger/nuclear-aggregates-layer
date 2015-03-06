using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201501190608, "Добавляем поле Amount в таблицу Billing.Charges", "y.baranihin")]
    public class Migration201501190608 : TransactedMigration
    {
        private const string TargetColumnName = "Amount";
        private readonly SchemaQualifiedObjectName _targetTableName = ErmTableNames.Charges;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(_targetTableName);
            if (table.Columns.Contains(TargetColumnName))
            {
                return;
            }

            InsertColumn(context, table);
            FillColumnWithValues(context.Connection);
            SetColumnNotNullable(context.Database.GetTable(_targetTableName));
        }

        private void InsertColumn(IMigrationContext context, Table table)
        {
            const int TargetColumnIndex = 7;
            var columnsToInsert = new[] { new InsertedColumnDefinition(TargetColumnIndex, x => new Column(x, TargetColumnName, DataType.Decimal(2, 9)) { Nullable = true }) };
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }

        private void FillColumnWithValues(ServerConnection connection)
        {
            const string Query = @"Update c set c.Amount = pp.Cost
                                    from Billing.Charges c 
                                    join Billing.OrderPositions op on c.OrderPositionId = op.Id 
                                    join Billing.PricePositions pp on op.PricePositionId = pp.Id";

            connection.ExecuteNonQuery(Query);
        }

        private void SetColumnNotNullable(Table table)
        {
            var column = table.Columns[TargetColumnName];
            column.Nullable = false;
            column.Alter();
        }
    }
}