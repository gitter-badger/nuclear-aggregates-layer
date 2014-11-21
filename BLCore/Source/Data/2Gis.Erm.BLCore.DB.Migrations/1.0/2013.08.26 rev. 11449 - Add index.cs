using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11449, "Добавляем индекс")]
    public sealed class Migration11449 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["Categories", ErmSchemas.BusinessDirectory];

            var index = table.Indexes["IX_Categories_ParentId"];
            if (index != null)
            {
                return;
            }

            index = new Index(table, "IX_Categories_ParentId");
            index.IndexedColumns.Add(new IndexedColumn(index, "ParentId"));

            index.Create();
        }
    }
}