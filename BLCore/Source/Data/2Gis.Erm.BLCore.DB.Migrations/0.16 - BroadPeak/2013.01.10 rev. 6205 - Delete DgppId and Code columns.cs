using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6205, "Удаляем колонки DgppId и Code в таблицах AssociatedPositions, AssociatedPositionsGroups и DeniedPositions")]
    public sealed class Migration6205 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table1 = context.Database.Tables["AssociatedPositions", ErmSchemas.Billing];
            DropColumns(table1);

            var table2 = context.Database.Tables["AssociatedPositionsGroups", ErmSchemas.Billing];
            DropColumns(table2);

            var table3 = context.Database.Tables["DeniedPositions", ErmSchemas.Billing];
            DropColumns(table3);
        }

        private static void DropColumns(TableViewTableTypeBase table)
        {
            var column1 = table.Columns["DgppId"];
            if (column1 != null)
            {
                column1.Drop();
            }

            var column2 = table.Columns["Code"];
            if (column2 != null)
            {
                column2.Drop();
            }
        }
    }
}