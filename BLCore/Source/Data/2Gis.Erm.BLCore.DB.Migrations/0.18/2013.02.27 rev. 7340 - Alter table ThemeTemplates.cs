using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7340, "Добавлена колонка [IsSkyScrapper] в [Billing].[ThemeTemplates]")]
    public sealed class Migration7340 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.ThemeTemplates);

            if (table.Columns.Contains("IsSkyScrapper"))
            {
                return;
            }

            var columnsToInsert = new[]
                {
                    new InsertedColumnDefinition(3, 
                        x => 
                        {
                            var c = new Column(x, "IsSkyScrapper", DataType.Bit) { Nullable = false };
                            c.AddDefaultConstraint().Text = "0";
                            return c;
                        })
                };
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }
    }
}
