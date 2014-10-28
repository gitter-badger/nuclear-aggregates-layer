using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25519, "Удаляем Description в [Shared].[TimeZones]", "y.baranihin")]
    public class Migration25519 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.TimeZones);
            var columnToDrop = table.Columns["Description"];
            if (columnToDrop != null)
            {
                columnToDrop.Drop();
            }
        }
    }
}