using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8984, "Переименование колонок в BusinessOperationServices")]
    public class Migration8984 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.BusinessOperationServices.Name, ErmTableNames.BusinessOperationServices.Schema];
            RenameColumn(table, "OperationName", "Operation");
            RenameColumn(table, "EntityName", "Descriptor");
        }

        private void RenameColumn(Table table, string oldName, string newName)
        {
            var column = table.Columns[oldName];
            if (column == null)
            {
                return;
            }

            column.Rename(newName);
            column.Alter();
        }
    }
}
