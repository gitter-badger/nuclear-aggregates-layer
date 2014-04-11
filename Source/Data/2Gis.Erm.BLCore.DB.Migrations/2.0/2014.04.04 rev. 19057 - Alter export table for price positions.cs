using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(19057, "Переименование интеграционной таблицы ExportFlowPriceLists_PricePosition => ExportFlowPriceLists_PriceListPosition", "d.ivanov")]
    public class Migration19057 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowPriceListsPricePosition, ErmTableNames.ExportFlowPriceListsPriceListPosition);
        }

        private static void CreateServiceBusExportTable(IMigrationContext context, SchemaQualifiedObjectName oldTableName, SchemaQualifiedObjectName tableName)
        {
            var table = context.Database.Tables[oldTableName.Name, oldTableName.Schema];
            if (table == null)
            {
                return;
            }

            table.Rename(tableName.Name);
            table.Alter();
        }
    }
}
