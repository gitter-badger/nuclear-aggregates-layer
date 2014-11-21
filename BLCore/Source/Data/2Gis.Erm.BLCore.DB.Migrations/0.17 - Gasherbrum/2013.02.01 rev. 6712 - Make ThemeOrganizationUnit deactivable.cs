using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6712, "Добавление колонки IsActive в сущность ThemeOrganizationUnit")]
    public class Migration6712 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.ThemeOrganizationUnits);

            if (table.Columns.Contains("IsActive"))
            {
                return;
            }


            var columnsToInsert = new[]
                {
                    new InsertedColumnDefinition(3, 
                        x =>
                        {
                            var c = new Column(x, "IsActive", DataType.Bit) { Nullable = false };
                            c.AddDefaultConstraint().Text = "1";
                            return c;
                        })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }
    }
}