using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._01___Praha
{
    [Migration(12176, "Добавление специфичных полей для Праги - 2")]
    public sealed class Migration12176 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var legalPersonsProfilesTable = context.Database.GetTable(ErmTableNames.LegalPersonProfiles);

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(12, smo => new Column(smo, "BankCode", DataType.NVarChar(4)) { Nullable = true }),
                    new InsertedColumnDefinition(6, smo => new Column(smo, "Registered", DataType.NVarChar(150)) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, legalPersonsProfilesTable, newColumns);
        }
    }
}
