using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5724, "Делаем колонку OperationTypes.IsDeleted not nullable")]
    public class Migration5724 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["OperationTypes", ErmSchemas.Billing];
            var column = table.Columns["IsDeleted"];
            column.Nullable = false;
            column.Alter();
        }
    }
}