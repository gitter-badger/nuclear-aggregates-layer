using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(11058, "Делаем колонки Error, Status в таблице AdvertisementElements не nullable")]
    public sealed class Migration11058 : TransactedMigration
    {
        private const string FixupQuery = "update Billing.AdvertisementElements set Error = 0, Status = 0 where Error is null or Status is null";
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(FixupQuery);
            var advertisementElements = context.Database.GetTable(ErmTableNames.AdvertisementElements);
            var errorColumn = advertisementElements.Columns["Error"];
            var statusColumn = advertisementElements.Columns["Status"];

            if (errorColumn != null)
            {
                errorColumn.Nullable = false;
                errorColumn.Alter();
            }

            if (statusColumn != null)
            {
                statusColumn.Nullable = false;
                statusColumn.Alter();
            }
        }
    }
}