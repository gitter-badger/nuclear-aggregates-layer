using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502190640, "Rename AdditionalEmail to Email", "y.baranihin")]
    public class Migration201502190640 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.LegalPersonProfiles);
            var columnToRename = table.Columns["AdditionalEmail"];
            if (columnToRename != null)
            {
                columnToRename.Rename("Email");
            }
        }
    }
}