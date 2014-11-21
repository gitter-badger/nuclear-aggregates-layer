using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21596, "Создание таблицы AdvertisementElementStatus", "a.rechkalov")]
    public class Migration21596 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var tableName = ErmTableNames.AdvertisementElementStatus;

            var table = context.Database.Tables[tableName.Name, tableName.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, tableName.Name, tableName.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("Status", DataType.Int, false);
            table.CreateAuditableEntityColumns();
            table.CreateTimestampColumn();

            table.Create();
            table.CreatePrimaryKey("Id");
            table.CreateForeignKey("Id", ErmTableNames.AdvertisementElements, "Id");
        }
    }
}