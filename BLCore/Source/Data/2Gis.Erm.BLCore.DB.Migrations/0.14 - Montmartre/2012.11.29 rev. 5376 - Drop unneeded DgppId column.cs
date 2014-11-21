using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5376, "Удаляем колонку DgppId из таблицы Billing.BranchOfficeOrganizationUnits")]
    public sealed class Migration5376 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropColumn(context);
        }

        private static void DropColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["BranchOfficeOrganizationUnits", ErmSchemas.Billing];
            var column = table.Columns["DgppId"];
            if (column == null)
                return;

            column.Drop();
        }
    }
}