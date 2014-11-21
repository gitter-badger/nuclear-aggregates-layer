using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6244, "Удаляем колонку CreateDate из таблицы CurrencyRates")]
    public sealed class Migration6244 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["CurrencyRates", ErmSchemas.Billing];

            var column = table.Columns["CreateDate"];
            if (column != null)
            {
                column.Drop();
            }
        }
    }
}