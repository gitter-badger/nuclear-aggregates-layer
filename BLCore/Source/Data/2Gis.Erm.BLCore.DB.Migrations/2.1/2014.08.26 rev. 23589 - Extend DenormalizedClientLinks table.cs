using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23589, "Расширение таблицы Billing.DenormalizedClientLinks", "y.baranikhin")]
    public class Migration23589 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.DenormalizedClientLinks);
            const string GraphKey = "GraphKey";

            if (table == null)
            {
                return;
            }

            var newColumns = new[]
                {
                    new InsertedColumnDefinition(4, x => new Column(x, GraphKey, DataType.UniqueIdentifier) { Nullable = false }),
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);
        }
    }
}