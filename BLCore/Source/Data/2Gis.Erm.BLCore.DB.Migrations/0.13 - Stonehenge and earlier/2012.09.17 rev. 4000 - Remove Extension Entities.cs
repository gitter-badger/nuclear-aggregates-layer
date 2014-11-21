using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4000, "Удаление Extension объектов")]
    public class Migration4000 : TransactedMigration
    {
        private class ColumnShortInfo
        {
            public ColumnShortInfo(string name, DataType dataType)
            {
                Name = name;
                DataType = dataType;
            }

            public String Name { get; private set; }
            public DataType DataType { get; private set; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.Tables.Contains("OrderExtensions", ErmSchemas.Billing))
            {
                MergeOrderExtensionsTable(context);
            }
            if (context.Database.Tables.Contains("DealExtensions", ErmSchemas.Billing))
            {
                MergeDealExtensionsTable(context);
            }

            SchemaQualifiedObjectName replicateOrderProc = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateOrder");
            ReplicationHelper.UpdateStoredProcUsingAttachedTemplate(context, this, replicateOrderProc, "Migration4000ReplicateOrderTemplate");

            SchemaQualifiedObjectName replicateDealProc = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateDeal");
            ReplicationHelper.UpdateStoredProcUsingAttachedTemplate(context, this, replicateDealProc, "Migration4000ReplicateDealTemplate");

            var sp = context.Database.StoredProcedures["ReplicateDealExtension", ErmSchemas.Billing];
            if (sp != null)
                sp.Drop();
            sp = context.Database.StoredProcedures["ReplicateOrderExtension", ErmSchemas.Billing];
            if (sp != null)
                sp.Drop();
        }

        private static void MergeOrderExtensionsTable(IMigrationContext context)
        {
            var ordersTable = context.Database.GetTable(ErmTableNames.Orders);

            DataType ermMoneyDecimal = DataType.Decimal(4, 19);

            List<InsertedColumnDefinition> columnsToInsert = new List<InsertedColumnDefinition>();
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "PayablePrice", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "PayablePlan", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "PayableFact", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "DiscountSum", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "DiscountPercent", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "VatPlan", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "AmountToWithdraw", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "AmountWithdrawn", ermMoneyDecimal)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "BudgetType", DataType.Int)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "InspectorCode", DataType.Int)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "Comment", DataType.NVarChar(300))));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "OrderType", DataType.Int)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "TerminationReason", DataType.Int)));
            columnsToInsert.Add(new InsertedColumnDefinition(32, x => new Column(x, "PlatformId", DataType.Int)));

            ordersTable = EntityCopyHelper.CopyAndInsertColumns(context.Database, ordersTable, columnsToInsert);
            const String updateOrdersQuery =
                @"
GO

UPDATE Billing.Orders SET 
PayablePrice = oe.PayablePrice,
PayablePlan = oe.PayablePlan,
PayableFact = oe.PayableFact,
DiscountSum = oe.DiscountSum,
DiscountPercent = oe.DiscountPercent,
VatPlan = oe.VatPlan,
AmountToWithdraw = oe.AmountToWithdraw,
AmountWithdrawn = oe.AmountWithdrawn,
BudgetType = oe.BudgetType,
InspectorCode = oe.InspectorCode,
Comment = oe.Comment,
OrderType = oe.OrderType,
TerminationReason = oe.TerminationReason,
PlatformId = oe.PlatformId
FROM Billing.OrderExtensions oe
WHERE Billing.Orders.Id = oe.Id
";

            context.Connection.ExecuteNonQuery(updateOrdersQuery);

            // После заливки данных можем развешивать NOT Null.
            ordersTable.SetNonNullableColumns("PayablePrice", "PayablePlan", "PayableFact", "VatPlan", "AmountToWithdraw", "AmountWithdrawn",
                                              "BudgetType", "OrderType", "TerminationReason");
            ordersTable.Alter();

            ForeignKey fKey = new ForeignKey(ordersTable, "FK_Orders_Platforms")
                {
                    ReferencedTableSchema = ErmSchemas.Billing,
                    ReferencedTable = "Platforms",
                };

            fKey.Columns.Add(new ForeignKeyColumn(fKey, "PlatformId", "Id"));
            fKey.Create();

            context.Database.Tables["OrderExtensions", ErmSchemas.Billing].Drop();
        }

        private static void MergeDealExtensionsTable(IMigrationContext context)
        {
            var dealsTable = context.Database.GetTable(ErmTableNames.Deals);

            List<InsertedColumnDefinition> columnsToInsert = new List<InsertedColumnDefinition>();
            columnsToInsert.Add(new InsertedColumnDefinition(11, x => new Column(x, "EstimatedProfit", DataType.Decimal(2, 16))));
            columnsToInsert.Add(new InsertedColumnDefinition(11, x => new Column(x, "ActualProfit", DataType.Decimal(2, 16))));
            columnsToInsert.Add(new InsertedColumnDefinition(11, x => new Column(x, "DealStage", DataType.Int)));

            dealsTable = EntityCopyHelper.CopyAndInsertColumns(context.Database, dealsTable, columnsToInsert);
            const String updateDealsQuery =
@"
GO

UPDATE Billing.Deals SET 
EstimatedProfit = de.EstimatedProfit,
ActualProfit = de.ActualProfit,
DealStage = de.DealStage
FROM Billing.DealExtensions de
WHERE Billing.Deals.Id = de.Id
";
            context.Connection.ExecuteNonQuery(updateDealsQuery);

            // После заливки данных можем развешивать NOT Null.
            dealsTable.Columns["EstimatedProfit"].Nullable = false;
            dealsTable.Columns["ActualProfit"].Nullable = false;
            dealsTable.Columns["DealStage"].Nullable = false;
            dealsTable.Alter();

            context.Database.Tables["DealExtensions", ErmSchemas.Billing].Drop();

        }
    }
}
