using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5752, "Добавлены колонки в Billing.Positions и Billing.PricePositions")]
    public sealed class Migration5752 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var positionsTable = context.Database.GetTable(ErmTableNames.Positions);

            if (positionsTable.Columns.Contains("IsControlledByAmount"))
            {
                return;
            }

            var columnsToInsert = new List<InsertedColumnDefinition>
                                      {
                                          new InsertedColumnDefinition(
                                              5, x => new Column(x, "IsControlledByAmount", DataType.Bit) { Nullable = true })
                                      };

            var alteredTable = EntityCopyHelper.CopyAndInsertColumns(context.Database, positionsTable, columnsToInsert);

            const string UpdatePositionsQuery = @"UPDATE Billing.Positions SET IsControlledByAmount = 0";

            context.Connection.ExecuteNonQuery(UpdatePositionsQuery);

            // После заливки данных можем развешивать NOT Null.
            alteredTable.SetNonNullableColumns("IsControlledByAmount");
            alteredTable.Alter();

            var pricePositionsTable = context.Database.GetTable(ErmTableNames.PricePositions);

            if (pricePositionsTable.Columns.Contains("MinAdvertisementAmount"))
            {
                return;
            }

            columnsToInsert = new List<InsertedColumnDefinition>
                                  {
                                      new InsertedColumnDefinition(
                                          5, x => new Column(x, "MinAdvertisementAmount", DataType.Int) { Nullable = true }),
                                      new InsertedColumnDefinition(
                                          5, x => new Column(x, "MaxAdvertisementAmount", DataType.Int) { Nullable = true })
                                  };

            alteredTable = EntityCopyHelper.CopyAndInsertColumns(context.Database, pricePositionsTable, columnsToInsert);
            alteredTable.Alter();
        }
    }
}
