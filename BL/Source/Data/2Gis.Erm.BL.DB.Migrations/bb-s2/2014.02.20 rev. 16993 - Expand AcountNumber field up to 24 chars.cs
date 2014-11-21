using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(16993, "Увиличиваем ширину поля AccountNumber с 16 до 24 символов", "a.rechkalov")]
    public sealed class Migration16993 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.LegalPersonProfiles);
            var column = table.Columns["AccountNumber"];
            column.DataType = Microsoft.SqlServer.Management.Smo.DataType.NVarChar(24);

            table.Alter();
        }
    }
}
