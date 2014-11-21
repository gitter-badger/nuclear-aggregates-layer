using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4602, "Удалёем колонку OwnerCode из Shared.Operations")]
    public sealed class Migration4602 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["Operations", ErmSchemas.Shared];

            var column = table.Columns["OwnerCode"];
            if (column != null)
                column.Drop();

        }
    }
}
