using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;
namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9083, "Добавляем колонки Status и Error в таблицу AdvertisementElements")]
    public sealed class Migration9083 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.AdvertisementElements);

            var columnsToInsert = new List<InsertedColumnDefinition>();

            if (!table.Columns.Contains("Status"))
            {
                columnsToInsert.Add(new InsertedColumnDefinition(7, x => new Column(x, "Status", DataType.Int) { Nullable = true }));
            }

            if (!table.Columns.Contains("Error"))
            {
                columnsToInsert.Add(new InsertedColumnDefinition(7, x => new Column(x, "Error", DataType.Int) { Nullable = true }));
            }
            
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }
    }
}