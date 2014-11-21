using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11308, "Advertisement templates проверяем что колонка DummyAdvertisementId имеет тип bigint")]
    public class Migration11308 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.AdvertisementTemplates.Name, ErmTableNames.AdvertisementTemplates.Schema];
            var column = table.Columns["DummyAdvertisementId"];

            if (column.DataType.SqlDataType == SqlDataType.BigInt)
            {
                return;
            }

            column.DataType = new DataType(SqlDataType.BigInt);
            column.Alter();
        }
    }
}
