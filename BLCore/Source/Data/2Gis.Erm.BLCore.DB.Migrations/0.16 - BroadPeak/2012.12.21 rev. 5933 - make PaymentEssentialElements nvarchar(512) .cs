using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5933, "Увеличиваем длину платежных реквизитов до 512 символов")]
    public sealed class Migration5933 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.LegalPersonProfiles);

            var column = table.Columns["PaymentEssentialElements"];
            if(column == null)
                return;

            column.DataType = DataType.NVarChar(512);
            column.Alter();
        }
    }
}
