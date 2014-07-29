using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4352, "Поле Operations.FileId стало nullable.")]
    public class Migration4352 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.Operations);
            table.Columns["FileId"].Nullable = true;
            table.Columns["FileId"].Rename("LogFileId");
            context.Database.GetTable(ErmTableNames.Operations).Alter();
        }
    }
}
