using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // В этой миграции после переноса в Main заменены типы идентификаторов. 
    // Обоснование: Если эта миграция была применена до 7778, то её идентификаторы будут обновлены миграциями перехода на int64,
    //              иначе идентификаторы сразу должны быть int64
    [Migration(9389, "Добавление таблицы учёта сущностей, не прошедших экспорт")]
    public class Migration9389 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateServiceBusExportTable(context, ErmTableNames.ExportFailedEntities);
        }

        private void CreateServiceBusExportTable(IMigrationContext context, SchemaQualifiedObjectName tableName)
        {
            var table = context.Database.Tables[tableName.Name, tableName.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, tableName.Name, tableName.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.BigInt) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("EntityName", DataType.Int, false);
            table.CreateField("EntityId", DataType.BigInt, false);

            table.Create();
            table.CreatePrimaryKey("Id");
        }
    }
}
