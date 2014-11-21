using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(16180, "Расширения поля RatePricePositions до enum", "a.tukaev")]
    public class Migration16180 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var pricePositions = context.Database.GetTable(ErmTableNames.PricePositions);

            var column = pricePositions.Columns["RatePricePositions"];
            if (column == null)
            {
                return;
            }

            column.Rename("RateType");
            column.DataType = DataType.Int;
            column.Alter();
        }
    }
}