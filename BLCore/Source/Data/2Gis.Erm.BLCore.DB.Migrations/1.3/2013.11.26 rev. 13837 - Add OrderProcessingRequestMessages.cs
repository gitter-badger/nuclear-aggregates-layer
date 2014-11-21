using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(13837, "Создаем таблицу сообщений по OrderProcessingRequests", "y.baranihin")]
    public class Migration13837 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string orderProcessingRequestMessages = "OrderProcessingRequestMessages";
            const string idField = "Id";

            var table = context.Database.Tables[orderProcessingRequestMessages, ErmSchemas.Billing];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, orderProcessingRequestMessages, ErmSchemas.Billing);

            table.Columns.Add(new Column(table, idField, DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "OrderRequestId", DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "MessageType", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "MessageTemplateCode", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "MessageParameters", DataType.Xml(string.Empty)) { Nullable = false });
            table.Columns.Add(new Column(table, "IsActive", DataType.Bit) { Nullable = false });
            table.Columns.Add(new Column(table, "CreatedBy", DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "CreatedOn", DataType.DateTime2(2)) { Nullable = false });
            table.Columns.Add(new Column(table, "ModifiedBy", DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, "ModifiedOn", DataType.DateTime2(2)) { Nullable = true });
            table.Columns.Add(new Column(table, "Timestamp", DataType.Timestamp) { Nullable = false });

            var primaryKeyIndex = new Index(table, "PK_OrderProcessingRequestMessages") { IndexKeyType = IndexKeyType.DriPrimaryKey };
            primaryKeyIndex.IndexedColumns.Add(new IndexedColumn(primaryKeyIndex, idField));
            table.Indexes.Add(primaryKeyIndex);

            table.Create();

            CreateFk(table, "FK_OrderProcessingRequestMessages_OrderProcessingRequests", "OrderRequestId", ErmTableNames.OrderProcessingRequests);
        }

        private static void CreateFk(Table table, string fkName, string fieldName, SchemaQualifiedObjectName ermTableName)
        {
            var foreignKey = new ForeignKey(table, fkName);
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, fieldName, "Id"));
            foreignKey.ReferencedTable = ermTableName.Name;
            foreignKey.ReferencedTableSchema = ermTableName.Schema;
            foreignKey.Create();
        }
    }
}
