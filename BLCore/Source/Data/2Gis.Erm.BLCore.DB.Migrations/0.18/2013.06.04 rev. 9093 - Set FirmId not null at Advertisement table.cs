using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9093, "Делаем идентификатор фирмы необязательным в таблице Advertisement")]
    public sealed class Migration9093 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.Advertisements.Name, ErmTableNames.Advertisements.Schema];
            var column = table.Columns["FirmId"];

            if (column.Nullable)
            {
                return;
            }

            column.Nullable = true;
            column.Alter();
        }
    }
}
