using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration
{
    [Migration(10786, "Удаляем колонку ShowFlampLink из Firms (задача ERM-227)")]
    public sealed class Migration10786 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["Firms", ErmSchemas.BusinessDirectory];

            DropCheckConstraint(table);
            DropColumn(table);
        }

        private static void DropColumn(Table table)
        {
            var column = table.Columns["ShowFlampLink"];
            if (column == null)
            {
                return;
            }

            column.Drop();
        }

        private static void DropCheckConstraint(Table table)
        {
            var check = table.Checks["CK_FirmsShowFlampLink"];
            if (check == null)
            {
                return;
            }

            check.Drop();
        }
    }
}