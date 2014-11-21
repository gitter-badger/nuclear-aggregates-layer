using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9140, "Добавление семи таблиц экспорта и удаление одной существующей")]
    public class Migration9140 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowOrdersAdvMaterial);
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowOrdersResource);
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowOrdersTheme);
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowOrdersThemeBranch);
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowFinancialDataLegalEntity);
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowOrdersOrder);
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowCardExtensionsCardCommercial);

            var table = context.Database.Tables[ErmTableNames.ServiceBusExportedBusinessOperations.Name, ErmTableNames.ServiceBusExportedBusinessOperations.Schema];
            if (table != null)
            {
                table.Drop();
            }
        }

        private void CreateServiceBusExportTable(IMigrationContext context, SchemaQualifiedObjectName tableName)
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
            table.CreateForeignKey("Id", ErmTableNames.PerformedBusinessOperations, "Id");
        }
    }
}
