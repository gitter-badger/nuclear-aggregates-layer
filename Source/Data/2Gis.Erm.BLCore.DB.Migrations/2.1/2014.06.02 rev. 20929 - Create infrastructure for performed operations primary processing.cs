using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20929, "Создание таблицы [Shared].[PerformedOperationsPrimaryProcessing]", "i.maslennikov")]
    public sealed class Migration20929 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetTableName = ErmTableNames.PerformedOperationPrimaryProcessings;
            var table = context.Database.Tables[targetTableName.Name, targetTableName.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, targetTableName.Name, targetTableName.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("MessageFlowId", DataType.UniqueIdentifier, false);
            table.CreateField("Date", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey(new[] { "Id", "MessageFlowId" });
        }
    }
}
