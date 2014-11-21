using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9862, "Добавление таблицы ExportToMsCrm_HotClients")]
    public class Migration9862 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateMsCrmExportTable(context, ErmTableNames.ExportToMsCrmHotClients);
        }

        private void CreateMsCrmExportTable(IMigrationContext context, SchemaQualifiedObjectName tableName)
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
            table.CreateForeignKey("Id", ErmTableNames.PerformedBusinessOperations, "Id");
        }
    }
}
