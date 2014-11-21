using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11484, "Добавляем индексы")]
    public sealed class Migration11484 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddIndex1(context);
            AddIndex2(context);
        }

        private static void AddIndex1(IMigrationContext context)
        {
            var table = context.Database.Tables["AssociatedPositionsGroups", ErmSchemas.Billing];

            var index = table.Indexes["IX_AssociatedPositionsGroups_PricePositionId"];
            if (index != null)
            {
                return;
            }

            index = new Index(table, "IX_AssociatedPositionsGroups_PricePositionId");
            index.IndexedColumns.Add(new IndexedColumn(index, "PricePositionId"));

            index.Create();
        }

        private static void AddIndex2(IMigrationContext context)
        {
            var table = context.Database.Tables["Positions", ErmSchemas.Billing];

            var index = table.Indexes["IX_Positions_CategoryId"];
            if (index != null)
            {
                return;
            }

            index = new Index(table, "IX_Positions_CategoryId");
            index.IndexedColumns.Add(new IndexedColumn(index, "CategoryId"));

            index.Create();
        }
    }
}