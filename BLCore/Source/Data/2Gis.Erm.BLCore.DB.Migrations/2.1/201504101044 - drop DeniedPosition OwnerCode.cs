using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201504101044, "Удаление OwnerCode у DeniedPosition", "y.baranihin")]
    public class Migration201504101044 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.DeniedPositions);
            var columnToDelete = table.Columns["OwnerCode"];
            if (columnToDelete != null)
            {
                columnToDelete.Drop();
            }
        }
    }
}