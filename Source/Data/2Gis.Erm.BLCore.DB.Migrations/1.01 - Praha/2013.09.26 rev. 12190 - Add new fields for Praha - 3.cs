using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._01___Praha
{
    [Migration(12190, "Добавление специфичных полей для Праги - 3")]
    public sealed class Migration12190 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var branchOfficeOrganizationUnitsTable = context.Database.GetTable(ErmTableNames.BranchOfficeOrganizationUnits);

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(11, smo => new Column(smo, "Registered", DataType.NVarChar(150)) { Nullable = true }),
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, branchOfficeOrganizationUnitsTable, newColumns);
        }
    }
}
