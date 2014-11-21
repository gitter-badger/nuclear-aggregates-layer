using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4193, "Удаление колонок IsActive и IsDeleted из таблицы Shared.Files")]
    public sealed class Migration4193 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropColumns(context);
        }

        private static void DropColumns(IMigrationContext context)
        {
            var table = context.Database.Tables["Files", ErmSchemas.Shared];

            DropIsActiveColumn(table);
            DropIsDeletedColumn(table);
        }

        private static void DropIsActiveColumn(TableViewTableTypeBase table)
        {
            var isActiveColumn = table.Columns["IsActive"];
            if (isActiveColumn == null)
                return;

            isActiveColumn.Drop();
        }

        private static void DropIsDeletedColumn(TableViewTableTypeBase table)
        {
            var isDeletedColumn = table.Columns["IsDeleted"];
            if (isDeletedColumn == null)
                return;

            isDeletedColumn.Drop();
        }
    }
}
