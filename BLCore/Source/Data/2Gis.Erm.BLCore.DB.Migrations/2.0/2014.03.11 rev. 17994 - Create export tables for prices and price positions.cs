using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(17994, "Создание интеграционных таблиц ExportFlowPriceLists_PriceList и ExportFlowPriceLists_PricePosition", "d.ivanov")]
    public class Migration17994 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowPriceListsPriceList);
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowPriceListsPricePosition);
        }

        private static void CreateServiceBusExportTable(IMigrationContext context, SchemaQualifiedObjectName tableName)
        {
            var table = context.Database.Tables[tableName.Name, tableName.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, tableName.Name, tableName.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("Success", DataType.Bit, false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey("Id");
        }
    }
}
