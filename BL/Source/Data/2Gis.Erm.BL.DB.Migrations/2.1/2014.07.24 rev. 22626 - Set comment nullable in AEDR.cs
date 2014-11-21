using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22626, "Делаем колонку Comment в таблице AdvertisementElementDenialReasons nullable", "y.baranihin")]
    public class Migration22626 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.AdvertisementElementDenialReason);

            var commentColumn = table.Columns["Comment"];
            commentColumn.Nullable = true;
            commentColumn.Alter();
        }
    }
}