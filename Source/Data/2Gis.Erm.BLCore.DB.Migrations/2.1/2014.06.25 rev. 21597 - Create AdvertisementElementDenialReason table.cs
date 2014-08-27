using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21597, "Создание таблицы AdvertisementElementDenialReason", "a.rechkalov")]
    public class Migration21597 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var tableName = ErmTableNames.AdvertisementElementDenialReason;

            var table = context.Database.Tables[tableName.Name, tableName.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, tableName.Name, tableName.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("AdvertisementElementId", DataType.BigInt, false);
            table.CreateField("DenialReasonId", DataType.BigInt, false);
            table.CreateField("Comment", DataType.NVarCharMax, false);
            table.CreateAuditableEntityColumns();
            table.CreateTimestampColumn();

            table.Create();
            table.CreatePrimaryKey(new[] { "Id" });
            table.CreateForeignKey("AdvertisementElementId", ErmTableNames.AdvertisementElements, "Id");
            table.CreateForeignKey("DenialReasonId", ErmTableNames.DenialReasons, "Id");
        }
    }
}