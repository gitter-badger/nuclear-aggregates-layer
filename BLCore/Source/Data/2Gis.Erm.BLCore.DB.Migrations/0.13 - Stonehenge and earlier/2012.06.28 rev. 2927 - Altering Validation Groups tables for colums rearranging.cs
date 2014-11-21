using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2927, "Изменение таблиц для реализации групп проверок заказов: [Billing].[OrderValidationRuleGroups], " +
                            "[Billing].[OrderValidationRuleGroupDetails] - изменение типов колонок имен и кодов")]
    public class Migration2927 : TransactedMigration
    {
    	protected override void ApplyOverride(IMigrationContext context)
		{
            if (context.Database.Tables.Contains(ErmTableNames.OrderValidationRuleGroups.Name, ErmTableNames.OrderValidationRuleGroups.Schema))
            {
                AlterOrderValidationRuleGroupsTable(context.Database);
            }
            if (context.Database.Tables.Contains(ErmTableNames.OrderValidationRuleGroupDetails.Name, ErmTableNames.OrderValidationRuleGroupDetails.Schema))
            {
                AlterOrderValidationRuleGroupDetailsTable(context.Database);
            }
		}

        private static void AlterOrderValidationRuleGroupsTable(Database database)
        {
            var table = database.Tables[ErmTableNames.OrderValidationRuleGroups.Name, ErmTableNames.OrderValidationRuleGroups.Schema];
            if (table.Checks.Contains("CK_OrderValidationRuleGroups_NameIsNotEmpty"))
            {
                var nameIsNotEmptyCheck = table.Checks["CK_OrderValidationRuleGroups_NameIsNotEmpty"];
                nameIsNotEmptyCheck.Drop();
            }
            if (table.Columns.Contains("Name"))
            {
                var nameColumn = table.Columns["Name"];
                nameColumn.Drop();
            }
            if (!table.Columns.Contains("Code"))
            {
                table.Columns.Add(new Column(table, "Code", DataType.Int) {Nullable = false});
            }
            if (!table.Checks.Contains("CK_OrderValidationRuleGroups_CodeIsNotEmpty"))
            {
                table.Checks.Add(new Check(table, "CK_OrderValidationRuleGroups_CodeIsNotEmpty") {Text = "Code<>0"});
            }

            table.Alter();
        }

        private static void AlterOrderValidationRuleGroupDetailsTable(Database database)
        {
            var table = database.Tables[ErmTableNames.OrderValidationRuleGroupDetails.Name, ErmTableNames.OrderValidationRuleGroupDetails.Schema];
            if (table.Checks.Contains("CK_OrderValidationRuleGroupDetails_RuleCodeIsNotEmpty"))
            {
                var ruleCodeIsNotEmptyCheck = table.Checks["CK_OrderValidationRuleGroupDetails_RuleCodeIsNotEmpty"];
                ruleCodeIsNotEmptyCheck.Drop();
            }
            if (table.Columns.Contains("RuleCode"))
            {
                var ruleCodeColumn = table.Columns["RuleCode"];
                ruleCodeColumn.Drop();
            }

            table.Columns.Add(new Column(table, "RuleCode", DataType.Int) {Nullable = false});
            table.Checks.Add(new Check(table, "CK_OrderValidationRuleGroupDetails_RuleCodeIsNotEmpty") { Text = "RuleCode<>0" });

            table.Alter();
        }
	}
}