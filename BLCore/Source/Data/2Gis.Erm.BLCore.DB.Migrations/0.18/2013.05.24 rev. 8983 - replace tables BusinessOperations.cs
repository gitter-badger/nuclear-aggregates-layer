using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8983, "Замена таблиц бизнес-операций")]
    public class Migration8983 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropTable(context, ErmTableNames.ExportBusinessOperations);
            DropTable(context, ErmTableNames.BusinessOperations);

            CreatePerformedBusinessOperationsTable(context);
            CreateServiceBusExportedBusinessOperations(context);
        }

        private void CreateServiceBusExportedBusinessOperations(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.ServiceBusExportedBusinessOperations.Name, ErmTableNames.ServiceBusExportedBusinessOperations.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.ServiceBusExportedBusinessOperations.Name, ErmTableNames.ServiceBusExportedBusinessOperations.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("Success", DataType.Bit, false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey("Id");
            table.CreateForeignKey("Id", ErmTableNames.PerformedBusinessOperations, "Id");
        }

        private void CreatePerformedBusinessOperationsTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.PerformedBusinessOperations.Name, ErmTableNames.PerformedBusinessOperations.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.PerformedBusinessOperations.Name, ErmTableNames.PerformedBusinessOperations.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.BigInt) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("Operation", DataType.Int, false);
            table.CreateField("Descriptor", DataType.Int, false);
            table.CreateField("Context", DataType.Xml(string.Empty), false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey();
        }

        private void DropTable(IMigrationContext context, SchemaQualifiedObjectName tableName)
        {
            var table = context.Database.Tables[tableName.Name, tableName.Schema];
            if (table != null)
            {
                table.Drop();
            }
        }
    }
}
