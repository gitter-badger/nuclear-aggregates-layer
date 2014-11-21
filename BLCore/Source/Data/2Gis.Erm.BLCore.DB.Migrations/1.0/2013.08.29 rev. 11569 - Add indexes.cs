using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11569, "Добавляем индексы")]
    public sealed class Migration11569 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddIndex1(context);
            AddIndex2(context);
        }

        private static void AddIndex1(IMigrationContext context)
        {
            var table = context.Database.Tables["OrderPositionAdvertisement", ErmSchemas.Billing];

            var index = table.Indexes["IX_OrderPositionAdvertisement_OrderPositionId_PositionId"];
            if (index != null)
            {
                return;
            }

            index = new Index(table, "IX_OrderPositionAdvertisement_OrderPositionId_PositionId");
            index.IndexedColumns.Add(new IndexedColumn(index, "OrderPositionId"));
            index.IndexedColumns.Add(new IndexedColumn(index, "PositionId"));

            index.Create();
        }

        private static void AddIndex2(IMigrationContext context)
        {
            var table = context.Database.Tables["NotificationProcessings", ErmSchemas.Shared];

            var index = table.Indexes["IX_NotificationProcessings_EmailId"];
            if (index != null)
            {
                return;
            }

            index = new Index(table, "IX_NotificationProcessings_EmailId");
            index.IndexedColumns.Add(new IndexedColumn(index, "EmailId"));

            index.Create();
        }
    }
}