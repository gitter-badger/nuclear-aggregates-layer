using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6372, "Добавлена колонка IsLocatedOnTheMap в FirmAddresses")]
    public sealed class Migration6372 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var currentTimeout = context.Connection.StatementTimeout;
            context.Connection.StatementTimeout = 1200;
            const string ColumnName = "IsLocatedOnTheMap";
            var firmAddressesTable = context.Database.GetTable(ErmTableNames.FirmAddresses);

            if (firmAddressesTable.Columns.Contains(ColumnName))
            {
                return;
            }

            var columnsToInsert = new List<InsertedColumnDefinition>
                                      {
                                          new InsertedColumnDefinition(
                                              6, x => new Column(x, ColumnName, DataType.Bit) { Nullable = true })
                                      };

            var alteredTable = EntityCopyHelper.CopyAndInsertColumns(context.Database, firmAddressesTable, columnsToInsert);

            const string UpdatePositionsQuery = @"UPDATE BusinessDirectory.FirmAddresses SET IsLocatedOnTheMap = (case WHEN BuildingCode Is Null THEN 0 ELSE 1 END)";

            context.Connection.ExecuteNonQuery(UpdatePositionsQuery);

            // После заливки данных можем развешивать NOT Null.
            alteredTable.SetNonNullableColumns(ColumnName);
            alteredTable.Alter();
            context.Connection.StatementTimeout = currentTimeout;
        }
    }
}
