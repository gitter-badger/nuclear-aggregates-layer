using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._01___Praha
{
    [Migration(11934, "Добавление специфичных полей для Праги")]
    public sealed class Migration11934 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddBankAddressFieldToLegalPersonsProfile(context);

            AddIcFieldTo(ErmTableNames.LegalPersons, 12, context);
            AddIcFieldTo(ErmTableNames.BranchOffices, 6, context);
        }

        private static void AddBankAddressFieldToLegalPersonsProfile(IMigrationContext context)
        {
            var legalPersonsProfilesTable = context.Database.GetTable(ErmTableNames.LegalPersonProfiles);

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(15, smo => new Column(smo, "BankAddress", DataType.NVarChar(512)) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, legalPersonsProfilesTable, newColumns);
        }

        private static void AddIcFieldTo(SchemaQualifiedObjectName tableIdentity, int position, IMigrationContext context)
        {
            var table = context.Database.GetTable(tableIdentity);

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(position, smo => new Column(smo, "Ic", DataType.NVarChar(8)) { Nullable = true })
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }
    }
}
