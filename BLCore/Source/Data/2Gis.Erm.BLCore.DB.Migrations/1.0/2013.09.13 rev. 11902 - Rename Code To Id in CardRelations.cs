using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11902, "Переименовываем Code в Id в таблице CardRelations")]
    public class Migration11902 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.CardRelations);
            if (table.Columns["Id"] != null)
            {
                return;
            }

            var codeColumn = table.Columns["Code"];
            codeColumn.Rename("Id");
        }
    }
}
