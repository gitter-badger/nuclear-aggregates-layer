using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(19206, "Удаление колонки Success из таблиц ExportFlowPriceLists_PriceList и ExportFlowPriceLists_PriceListPosition", "d.ivanov")]
    public class Migration19206 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterServiceBusExportTable(context, ErmTableNames.ExportFlowPriceListsPriceList);
            AlterServiceBusExportTable(context, ErmTableNames.ExportFlowPriceListsPriceListPosition);
        }

        private static void AlterServiceBusExportTable(IMigrationContext context, SchemaQualifiedObjectName tableName)
        {
            var table = context.Database.Tables[tableName.Name, tableName.Schema];
            if (table == null)
            {
                return;
            }

            var successColumn = table.Columns["Success"];
            if (successColumn != null)
            {
                successColumn.Drop();
            }
        }
    }
}
