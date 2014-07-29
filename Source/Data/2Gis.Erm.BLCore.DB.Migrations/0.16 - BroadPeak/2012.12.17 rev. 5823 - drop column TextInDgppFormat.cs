using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5823, "Удаляем колонку TextInDgppFormat из таблицы AdvertisementElements")]
    public class Migration5823 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["AdvertisementElements", ErmSchemas.Billing];
            var column = table.Columns["TextInDgppFormat"];

            if (column == null)
                return;

            column.Drop();
        }
    }
}