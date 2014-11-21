using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1416, "Добавление в договор даты закрытия.")]
// ReSharper disable InconsistentNaming
    public class Migration_1416 : TransactedMigration
// ReSharper restore InconsistentNaming
    {
        private const String ClosedColumnName = "ClosedOn";
        private const String ValidBargainPeriodCheckName = "CK_ValidBargainPeriod";


        protected override void ApplyOverride(IMigrationContext context)
        {
            Table bargainsTable = context.Database.GetTable(ErmTableNames.Bargains);
            Column closeDateColumn = bargainsTable.Columns[ClosedColumnName];
            if (closeDateColumn == null)
            {
                closeDateColumn = new Column(bargainsTable, ClosedColumnName, DataType.DateTime2(2)) { Nullable = true };
                closeDateColumn.Create();
            }

            if(!bargainsTable.Checks.Contains(ValidBargainPeriodCheckName))
            {
                Check validPeriodCheck = new Check(bargainsTable, ValidBargainPeriodCheckName) { Text = "([ClosedOn] IS NULL OR [ClosedOn] >= [SignedOn])" };
                validPeriodCheck.Create();
            }
        }

        protected override void RevertOverride(IMigrationContext context)
        {
            Table bargainsTable = context.Database.GetTable(ErmTableNames.Bargains);
            if (bargainsTable.Columns.Contains(ClosedColumnName))
            {
                Column closeDateColumn = bargainsTable.Columns[ClosedColumnName];
                closeDateColumn.Drop();
            }

            if(bargainsTable.Checks.Contains(ValidBargainPeriodCheckName))
            {
                bargainsTable.Checks[ValidBargainPeriodCheckName].Drop();
            }
        }
    }
}
