using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20930, "Создание таблицы  [Shared].[PerformedOperationFinalProcessings]", "i.maslennikov")]
    public sealed class Migration20930 : TransactedMigration
    {
        private readonly SchemaQualifiedObjectName _targetTableName = ErmTableNames.PerformedOperationFinalProcessings;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables[_targetTableName.Name, _targetTableName.Schema];
            if (table != null)
            {
                return;
            }

            table = CreateTable(context);
            table.CreateIndex(false, "MessageFlowId", "AttemptCount");
        }

        private Table CreateTable(IMigrationContext context)
        {
            var table = new Table(context.Database, _targetTableName.Name, _targetTableName.Schema);

            table.CreateField("Id", DataType.BigInt, false);
            table.CreateField("MessageFlowId", DataType.UniqueIdentifier, false);
            table.CreateField("EntityTypeId", DataType.Int, false);
            table.CreateField("EntityId", DataType.BigInt, false);
            table.CreateField("Context", DataType.Xml(string.Empty), true);
            table.CreateField("AttemptCount", DataType.Int, false);
            table.CreateField("OperationId", DataType.UniqueIdentifier, false);
            table.CreateField("CreatedOn", DataType.DateTime2(2), false);

            table.Create();
            table.CreatePrimaryKey(new[] { "Id" });

            return table;
        }
    }
}
