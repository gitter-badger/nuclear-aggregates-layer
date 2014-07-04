using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._0._18
{
    // после одного из merge данная миграция была исключена из проекта, но осталась в системе контроля версий
    // + появилась конфликтующая миграция с таким же номером и она, видимо, и была применена
    // т.к. действия из данной миграции больше не актуальны, просто сделаем класс internal, исключив его из поля зрения мигратора
    [Migration(8782, "Создание таблицы BusinessOperations")]
    internal class Migration8782 : TransactedMigration 
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.BusinessOperations.Name, ErmTableNames.BusinessOperations.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.BusinessOperations.Name, ErmTableNames.BusinessOperations.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("OperationName", DataType.Int, false);
            table.CreateField("EntityName", DataType.Int, false);
            table.CreateField("EntityId", DataType.Int, false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey();
        }
    }
}
