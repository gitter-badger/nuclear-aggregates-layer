using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6666, "Добавлена колонка email Billing.BranchOfficeOrganizationUnits")]
    public sealed class Migration6666 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var branchOfficeOrganizationUnitsTable = context.Database.GetTable(ErmTableNames.BranchOfficeOrganizationUnits);

            if (branchOfficeOrganizationUnitsTable.Columns.Contains("Email"))
            {
                return;
            }

            var columnsToInsert = new List<InsertedColumnDefinition>
                                      {
                                          new InsertedColumnDefinition(
                                              17, x => new Column(x, "Email", DataType.NVarChar(64)) { Nullable = true })
                                      };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, branchOfficeOrganizationUnitsTable, columnsToInsert);
        }
    }
}
