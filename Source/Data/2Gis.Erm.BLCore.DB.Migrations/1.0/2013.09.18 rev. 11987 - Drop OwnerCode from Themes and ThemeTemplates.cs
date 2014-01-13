using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11987, "Удаляем OwnerCode в таблицах Themes и ThemeTemplates")]
    public class Migration11987 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var themesTable = context.Database.GetTable(ErmTableNames.Themes);
            if (themesTable != null)
            {
                var ownerCode = themesTable.Columns["OwnerCode"];
                if (ownerCode != null)
                {
                    ownerCode.Drop();
                }
            }

            var themeTemplatesTable = context.Database.GetTable(ErmTableNames.ThemeTemplates);
            if (themesTable != null)
            {
                var ownerCode = themeTemplatesTable.Columns["OwnerCode"];
                if (ownerCode != null)
                {
                    ownerCode.Drop();
                }
            }
        }
    }
}