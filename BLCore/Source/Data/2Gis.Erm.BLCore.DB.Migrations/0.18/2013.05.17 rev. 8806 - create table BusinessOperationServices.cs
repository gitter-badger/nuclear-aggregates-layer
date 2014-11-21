using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // В этой миграции после переноса в Main заменены типы идентификаторов. 
    // Обоснование: Если эта миграция была применена до 7778, то её идентификаторы будут обновлены миграциями перехода на int64,
    //              иначе идентификаторы сразу должны быть int64
    [Migration(8806, "Создание таблицы BusinessOperationServices")]
    public class Migration8806 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.BusinessOperationServices.Name, ErmTableNames.BusinessOperationServices.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.BusinessOperationServices.Name, ErmTableNames.BusinessOperationServices.Schema);

            table.CreateField("EntityName", DataType.Int, false);
            table.CreateField("OperationName", DataType.Int, false);
            table.CreateField("Service", DataType.Int, false);
            table.CreateField("ServiceParameter", DataType.NVarChar(255), true);

            table.Create();

            var primaryKey = new Index(table, "PK_" + table.Name);
            primaryKey.IndexedColumns.Add(new IndexedColumn(primaryKey, "EntityName"));
            primaryKey.IndexedColumns.Add(new IndexedColumn(primaryKey, "OperationName"));
            primaryKey.IndexedColumns.Add(new IndexedColumn(primaryKey, "Service"));
            primaryKey.IndexKeyType = IndexKeyType.DriPrimaryKey;
            primaryKey.Create();
        }
    }
}
