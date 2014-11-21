using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(8762, "Удаляем колонку ClientId из таблицы Limit")]
    public class Migration8762 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Limits);
            DropOwnerCode(table);
        }

        private void DropOwnerCode(Table table)
        {
            var column = table.Columns["ClientId"];

            if (column == null)
            {
                return;
            }

            const string indexName = "IX_Limits_ClientId_IsActive_IsDeleted";
            var index = table.Indexes[indexName];
            if (index != null)
            {
                index.Drop();
            }

            column.Drop();
        }
    }
}
