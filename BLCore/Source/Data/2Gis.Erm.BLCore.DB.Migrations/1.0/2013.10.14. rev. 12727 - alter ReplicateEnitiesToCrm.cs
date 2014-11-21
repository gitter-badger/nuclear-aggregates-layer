using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12727, "Добавляем параметр chunkSize в процедуру ReplicateEntitiesToCrm")]
    public class Migration12727 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var command = Resources.Migration12727;
            context.Connection.ExecuteNonQuery(command);
        }
    }
}