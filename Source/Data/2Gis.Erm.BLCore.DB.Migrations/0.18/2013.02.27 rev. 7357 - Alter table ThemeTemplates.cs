using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7357, "Исправлена опечатка в названии колонки [IsSkyScraper] в [Billing].[ThemeTemplates]")]
    public sealed class Migration7357 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.ThemeTemplates);
            var column = table.Columns["IsSkyScrapper"];
            column.Rename("IsSkyScraper");
            table.Alter();
        }
    }
}
