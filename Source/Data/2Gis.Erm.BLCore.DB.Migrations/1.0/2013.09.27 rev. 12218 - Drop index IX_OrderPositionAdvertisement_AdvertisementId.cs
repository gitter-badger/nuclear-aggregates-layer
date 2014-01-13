using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12218, "Удаление индекса IX_OrderPositionAdvertisement_AdvertisementId")]
    public sealed class Migration12218 : TransactedMigration
    {
        private const string IndexName = "IX_OrderPositionAdvertisement_AdvertisementId";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.OrderPositionAdvertisement);
            if (table.Indexes.Contains(IndexName))
            {
                var index = table.Indexes[IndexName];
                index.Drop();
            }
        }
    }
}
