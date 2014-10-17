using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25340, "Меняем формат хранения кэша проверки заказов", "i.maslennikov")]
    public sealed class Migration25340 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            DropOldSchemaTable(context);
            CreateWithNewSchema(context);
        }

        private void DropOldSchemaTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.OrderValidationResults.Name, ErmTableNames.OrderValidationResults.Schema];
            if (table == null)
            {
                return;
            }

            table.Drop();
        }

        private void CreateWithNewSchema(IMigrationContext context)
        {
            var table = new Table(context.Database, ErmTableNames.OrderValidationResults.Name, ErmTableNames.OrderValidationResults.Schema);

            const string OrderIdColumnName = "OrderId";
            const string ValidatorIdColumnName = "ValidatorId";
            const string ValidVersionColumnName = "ValidVersion";
            const string OperationIdColumnName = "OperationId";

            table.CreateField(OrderIdColumnName, DataType.BigInt, false);
            table.CreateField(ValidatorIdColumnName, DataType.Int, false);
            table.CreateField(ValidVersionColumnName, DataType.Binary(8), false);
            table.CreateField(OperationIdColumnName, DataType.UniqueIdentifier, false);

            table.Create();

            string primaryKeyIndexName = string.Join("_",
                                                     "PK",
                                                     ErmTableNames.OrderValidationResults.Name,
                                                     OrderIdColumnName,
                                                     ValidatorIdColumnName,
                                                     ValidVersionColumnName,
                                                     OperationIdColumnName);

            var primaryKey = new Index(table, primaryKeyIndexName);

            var keyColumn = new IndexedColumn(primaryKey, OrderIdColumnName);
            primaryKey.IndexedColumns.Add(keyColumn);
            keyColumn = new IndexedColumn(primaryKey, ValidatorIdColumnName);
            primaryKey.IndexedColumns.Add(keyColumn);
            keyColumn = new IndexedColumn(primaryKey, ValidVersionColumnName);
            primaryKey.IndexedColumns.Add(keyColumn);
            keyColumn = new IndexedColumn(primaryKey, OperationIdColumnName);
            primaryKey.IndexedColumns.Add(keyColumn);

            primaryKey.IsClustered = false;
            primaryKey.IndexKeyType = IndexKeyType.DriPrimaryKey;
            primaryKey.Create();
        }
    }
}
