using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7464, "Удаляем колонку OwnerCode из таблицы CurrencyRates")]
    public class Migration7464 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.CurrencyRates);
            DropOwnerCode(table);
        }

        private void DropOwnerCode(Table table)
        {
            var column = table.Columns["OwnerCode"];

            if (column == null)
            {
                return;
            }

            column.Drop();
        }
    }
}
