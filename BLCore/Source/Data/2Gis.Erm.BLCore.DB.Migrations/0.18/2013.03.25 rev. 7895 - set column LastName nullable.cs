using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7895, "Делаем фамилию необязательной в таблице Contacts")]
    public sealed class Migration7895 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            MakeFirmDgppIdNotNull(context);
        }

        private static void MakeFirmDgppIdNotNull(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.Contacts.Name, ErmTableNames.Contacts.Schema];
            var column = table.Columns["LastName"];

            if (column.Nullable)
            {
                return;
            }

            column.Nullable = true;
            column.Alter();
        }
    }
}
