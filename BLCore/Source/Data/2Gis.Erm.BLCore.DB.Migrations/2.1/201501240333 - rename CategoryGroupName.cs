using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201501240333, "Переименовываем CategoryGroupName", "y.baranihin")]
    public class Migration201501240333 : TransactedMigration
    {
        private const string CategoryGroupNameColumn = "CategoryGroupName";
        private const string NameColumn = "Name";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.CategoryGroups.Name, ErmTableNames.CategoryGroups.Schema];
            table.Columns[CategoryGroupNameColumn].Rename(NameColumn);
        }
    }
}