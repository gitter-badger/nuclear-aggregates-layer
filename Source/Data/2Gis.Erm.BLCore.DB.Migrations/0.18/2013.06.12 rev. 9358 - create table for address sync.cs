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
    [Migration(9358, "Добавление таблицы учёта обработки синхронизации адресов")]
    public class Migration9358 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            CreateServiceBusExportTable(context, ErmTableNames.ImportedFirmAddresses);
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
