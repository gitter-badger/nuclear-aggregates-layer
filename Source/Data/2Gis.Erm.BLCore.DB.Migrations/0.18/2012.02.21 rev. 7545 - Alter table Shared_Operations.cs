using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7545, "Таблица Shared.Operations. Изменен тип колонки Type smallint->int")]
    public sealed class Migration7545 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Operations);
            const string TargetColumnName = "Type";
            var column = table.Columns[TargetColumnName];
            if (column == null)
            {
                throw new InvalidOperationException("Can't find required column " + TargetColumnName);
            }

            if (column.DataType.SqlDataType == DataType.Int.SqlDataType)
            {   // do nothing
                return;
            }

            column.DataType = DataType.Int;
            column.Alter();
        }
    }
}
