using DoubleGis.Erm.DB.Migration.Base;
using DoubleGis.Erm.DB.Migration.Impl.Shared;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(8782, "Создание таблицы BusinessOperations")]
    public class Migration8782 : TransactedMigration
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
