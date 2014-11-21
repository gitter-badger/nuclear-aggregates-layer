using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._2
{
    [Migration(13016, "Создаем таблицу OrderProcessingRequests")]
    public class Migration13016 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const string orderProcessingRequests = "OrderProcessingRequests";
            const string idField = "Id";

            var table = context.Database.Tables[orderProcessingRequests, ErmSchemas.Billing];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, orderProcessingRequests, ErmSchemas.Billing);

            table.Columns.Add(new Column(table, idField, DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "RequestId", DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "ReplicationCode", DataType.UniqueIdentifier) { Nullable = false });
            table.Columns.Add(new Column(table, "Title", DataType.NVarChar(200)) { Nullable = false });
            table.Columns.Add(new Column(table, "DueDate", DataType.DateTime2(2)) { Nullable = false });
            table.Columns.Add(new Column(table, "BaseOrderId", DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, "RenewedOrderId", DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, "SourceOrganizationUnitId", DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, "BeginDistributionDate", DataType.DateTime2(2)) { Nullable = true });
            table.Columns.Add(new Column(table, "FirmId", DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, "LegalPersonId", DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, "LegalPersonProfileId", DataType.BigInt) { Nullable = true });
            table.Columns.Add(new Column(table, "Description", DataType.NVarCharMax) { Nullable = true });
            table.Columns.Add(new Column(table, "State", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "OwnerCode", DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "CreatedBy", DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "CreatedOn", DataType.DateTime2(2)) { Nullable = false });
            table.Columns.Add(new Column(table, "ModifiedBy", DataType.BigInt) { Nullable = false });
            table.Columns.Add(new Column(table, "ModifiedOn", DataType.DateTime2(2)) { Nullable = false });
            table.Columns.Add(new Column(table, "Timestamp", DataType.Timestamp) { Nullable = false });

            var primaryKeyIndex = new Index(table, "PK_OrderProcessingRequests") { IndexKeyType = IndexKeyType.DriPrimaryKey };
            primaryKeyIndex.IndexedColumns.Add(new IndexedColumn(primaryKeyIndex, idField));
            table.Indexes.Add(primaryKeyIndex);

            table.Create();
        }
    }
}
