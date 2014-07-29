using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6353, "Добавлена колонка в [Billing].[OrderPositionAdvertisement]")]
    public sealed class Migration6353 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.OrderPositionAdvertisement);

            if (table.Columns.Contains("ThemeId"))
                return;

            var columnsToInsert = new[] { new InsertedColumnDefinition(6, x => new Column(x, "ThemeId", DataType.Int) { Nullable = true }) };
            table = EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
            table.CreateForeignKey("ThemeId", ErmTableNames.Themes, "Id");
            table.Alter();
        }
    }
}
