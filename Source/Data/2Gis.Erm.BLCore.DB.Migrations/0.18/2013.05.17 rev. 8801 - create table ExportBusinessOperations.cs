using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // В этой миграции после переноса в Main заменены типы идентификаторов. 
    // Обоснование: Если эта миграция была применена до 7778, то её идентификаторы будут обновлены миграциями перехода на int64,
    //              иначе идентификаторы сразу должны быть int64
    [Migration(8801, "Создание таблицы ExportBusinessOperations")]
    public class Migration8801 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.ExportBusinessOperations.Name, ErmTableNames.ExportBusinessOperations.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.ExportBusinessOperations.Name, ErmTableNames.ExportBusinessOperations.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("Success", DataType.Bit, false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey("Id");
            table.CreateForeignKey("Id", ErmTableNames.BusinessOperations, "Id");
        }
    }
}
