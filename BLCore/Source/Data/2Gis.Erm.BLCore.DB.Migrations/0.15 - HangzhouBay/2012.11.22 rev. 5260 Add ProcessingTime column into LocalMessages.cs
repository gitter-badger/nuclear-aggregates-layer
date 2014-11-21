using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5260, "Добавлена колонка ProcessingTime в Shared.LocalMessages")]
    public sealed class Migration5260 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.LocalMessages);

            if (table.Columns.Contains("ProcessingTime"))
                return;

            var columnsToInsert = new List<InsertedColumnDefinition>();
            columnsToInsert.Add(new InsertedColumnDefinition(7, x => new Column(x, "ProcessingTime", DataType.BigInt) { Nullable = true }));
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }
    }
}
