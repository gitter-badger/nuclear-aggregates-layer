using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9081, "Добавляем колонку IsValidate в таблицу AdvertisementElementTemplates")]
    public sealed class Migration9081 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.AdvertisementElementTemplates);

            if (table.Columns.Contains("IsValidate"))
            {
                return;
            }

            var columnsToInsert = new[]
                {
                    new InsertedColumnDefinition(12, 
                        x =>
                        {
                            var c = new Column(x, "IsValidate", DataType.Bit) { Nullable = false };
                            c.AddDefaultConstraint().Text = "0";
                            return c;
                        })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }
    }
}