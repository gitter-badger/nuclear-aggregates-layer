using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // В этой миграции после переноса в Main заменены типы идентификаторов. 
    // Обоснование: Если эта миграция была применена до 7778, то её идентификаторы будут обновлены миграциями перехода на int64,
    //              иначе идентификаторы сразу должны быть int64
    [Migration(8787, "Создание таблицы BusinessOperations")]
    public class Migration8787 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.BusinessOperations.Name, ErmTableNames.BusinessOperations.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.BusinessOperations.Name, ErmTableNames.BusinessOperations.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.BigInt) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("OperationName", DataType.Int, false);
            table.CreateField("EntityName", DataType.Int, false);
            table.CreateField("EntityId", DataType.BigInt, false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey();
        }
    }
}
