using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4603, "Удаляем колонку OwnerCode из Shared.Files")]
    public sealed class Migration4603 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["Files", ErmSchemas.Shared];

            var column = table.Columns["OwnerCode"];
            if (column != null)
                column.Drop();

        }
    }
}
