using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2911, "Добавление таблиц для реализации групп проверок заказов: [Billing].[OrderValidationRuleGroups]," +
                            "[Billing].[OrderValidationRuleGroupDetails], [Billing].[OrderValidationResults]")]
    public class Migration2911 : TransactedMigration
    {
    	protected override void ApplyOverride(IMigrationContext context)
		{
            if (!context.Database.Tables.Contains(ErmTableNames.OrderValidationRuleGroups.Name, ErmTableNames.OrderValidationRuleGroups.Schema))
            {
                CreateOrderValidationRuleGroupsTable(context.Database);
            }
            if (!context.Database.Tables.Contains(ErmTableNames.OrderValidationRuleGroupDetails.Name, ErmTableNames.OrderValidationRuleGroupDetails.Schema))
            {
                CreateOrderValidationRuleGroupDetailsTable(context.Database);
            }
            if (!context.Database.Tables.Contains(ErmTableNames.OrderValidationResults.Name, ErmTableNames.OrderValidationResults.Schema))
            {
                CreateOrderValidationResultsTable(context.Database);
            }
		}

        private static void CreateOrderValidationRuleGroupsTable(Database database)
        {
            var table = new Table(database,
                                  ErmTableNames.OrderValidationRuleGroups.Name,
                                  ErmTableNames.OrderValidationRuleGroups.Schema);
            table.Columns.Add(new Column(table, "Id", DataType.Int)
                                  {
                                      Nullable = false,
                                      Identity = true,
                                      IdentityIncrement = 1,
                                      IdentitySeed = 1
                                  });
            table.Columns.Add(new Column(table, "Name", DataType.NVarChar(200)) { Nullable = false });

            table.Checks.Add(new Check(table, "CK_OrderValidationRuleGroups_NameIsNotEmpty") { Text = "Name<>''" });
            
            table.Create();
            table.CreatePrimaryKey();
        }

        private static void CreateOrderValidationRuleGroupDetailsTable(Database database)
        {
            var table = new Table(database,
                                  ErmTableNames.OrderValidationRuleGroupDetails.Name,
                                  ErmTableNames.OrderValidationRuleGroupDetails.Schema);
            table.Columns.Add(new Column(table, "Id", DataType.Int)
                                  {
                                      Nullable = false,
                                      Identity = true,
                                      IdentityIncrement = 1,
                                      IdentitySeed = 1
                                  });
            table.Columns.Add(new Column(table, "OrderValidationGroupId", DataType.Int) {Nullable = false});
            table.Columns.Add(new Column(table, "RuleCode", DataType.NVarChar(200)) { Nullable = false });

            table.Checks.Add(new Check(table, "CK_OrderValidationRuleGroupDetails_RuleCodeIsNotEmpty") { Text = "RuleCode<>''" });

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("OrderValidationGroupId", ErmTableNames.OrderValidationRuleGroups, "Id");
        }

        private static void CreateOrderValidationResultsTable(Database database)
        {
            var table = new Table(database,
                                  ErmTableNames.OrderValidationResults.Name,
                                  ErmTableNames.OrderValidationResults.Schema);
            table.Columns.Add(new Column(table, "Id", DataType.Int)
                                  {
                                      Nullable = false,
                                      Identity = true,
                                      IdentityIncrement = 1,
                                      IdentitySeed = 1
                                  });
            table.Columns.Add(new Column(table, "OrderId", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "OrderValidationId", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "OrderValidationGroupId", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "OrderValidationType", DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, "IsValid", DataType.Bit) { Nullable = false });

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("OrderId", ErmTableNames.Orders, "Id");
        }
	}
}