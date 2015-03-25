using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201502240101, "Добавление таблицы экспорта списаний в 1с через шину", "a.rechkalov")]
    public class Migration201502240101 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateServiceBusExportTable(context, ErmTableNames.ExportFlowFinancialDataDebitsInfoInitial);
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
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey("Id");
        }
    }
}