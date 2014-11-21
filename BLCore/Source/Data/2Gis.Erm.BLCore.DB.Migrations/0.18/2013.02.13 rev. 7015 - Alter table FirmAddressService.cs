using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7015, "Добавлена колонка [Timestamp] в [Billing].[FirmAddressServices]")]
    public sealed class Migration7015 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.FirmAddressServices);

            if (table.Columns.Contains("Timestamp"))
            {
                return;
            }
            
            var columnsToInsert = new[] { new InsertedColumnDefinition(4, x => new Column(x, "Timestamp", DataType.Timestamp)) };
            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, columnsToInsert);
        }
    }
}
