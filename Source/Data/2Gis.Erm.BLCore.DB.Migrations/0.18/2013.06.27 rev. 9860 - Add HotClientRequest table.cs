using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(9860, "Создание таблицы запросов на создание горячих клиентов")]
    public sealed class Migration9860 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddHotClientRequestsTable(context);
        }

        private void AddHotClientRequestsTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.HotClientRequests.Name, ErmTableNames.HotClientRequests.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.HotClientRequests.Name, ErmTableNames.HotClientRequests.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.BigInt) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("SourceCode", DataType.NVarChar(200), false);
            table.CreateField("UserCode", DataType.NVarChar(200), false);
            table.CreateField("UserName", DataType.NVarChar(200), false);
            table.CreateField("CreationDate", DataType.DateTime2(2), false);
            table.CreateField("ContactName", DataType.NVarChar(200), false);
            table.CreateField("ContactPhone", DataType.NVarChar(200), false);
            table.CreateField("Description", DataType.NVarCharMax, true);
            table.CreateField("CardCode", DataType.BigInt, true);
            table.CreateField("BranchCode", DataType.BigInt, true);
            table.CreateField("TaskId", DataType.UniqueIdentifier, true);
            table.CreateField("CreatedBy", DataType.BigInt, false);
            table.CreateField("CreatedOn", DataType.DateTime2(2), false);
            table.CreateField("ModifiedBy", DataType.BigInt, true);
            table.CreateField("ModifiedOn", DataType.DateTime2(2), true);
            table.CreateField("Timestamp", DataType.Timestamp, true);

            table.Create();
            table.CreatePrimaryKey();
        }
    }
}
