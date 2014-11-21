using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6090, "Добавлены колонки поддерживается экспортом в Billing.PositionCategories")]
    public sealed class Migration6090 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var positionCategoriesTable = context.Database.GetTable(ErmTableNames.PositionCategories);

            if (positionCategoriesTable.Columns.Contains("IsSupportedByExport"))
            {
                return;
            }

            var columnsToInsert = new List<InsertedColumnDefinition>
                                      {
                                          new InsertedColumnDefinition(
                                              3, x => new Column(x, "IsSupportedByExport", DataType.Bit) { Nullable = true })
                                      };

            var alteredTable = EntityCopyHelper.CopyAndInsertColumns(context.Database, positionCategoriesTable, columnsToInsert);

            const string UpdatePositionsQuery = @"UPDATE Billing.PositionCategories SET IsSupportedByExport = 0";

            context.Connection.ExecuteNonQuery(UpdatePositionsQuery);

            // После заливки данных можем развешивать NOT Null.
            alteredTable.SetNonNullableColumns("IsSupportedByExport");
            alteredTable.Alter();
        }
    }
}
