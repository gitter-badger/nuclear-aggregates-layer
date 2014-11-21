using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12415, "Добавление колонки Timestamp в таблицу Bargain")]
    public sealed class Migration12415 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var bargainTable = context.Database.GetTable(ErmTableNames.Bargains);

            if (bargainTable.Columns.Contains("Timestamp"))
            {
                return;
            }

            var columnsToInsert = new List<InsertedColumnDefinition>
                {
                    new InsertedColumnDefinition(
                        19, x => new Column(x, "Timestamp", DataType.Timestamp) { Nullable = false })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, bargainTable, columnsToInsert);
        }
    }
}
