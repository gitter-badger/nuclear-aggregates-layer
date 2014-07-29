using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2946, "Перемещение проверок 'RequiredWhiteListedAdvertisementSpecified' и 'SameAdvertisements' из группы 'AdvertisementMaterialsValidation' в 'Generic'. " +
        "Удаление колонки OrderValidationId из таблицы Billing.OrderValidationResults")]
    public class Migration2946 : TransactedMigration
    {
        private const string UpdateGroupCodeForGroupDetailStatement = @"
UPDATE [Billing].[OrderValidationRuleGroupDetails] 
SET [OrderValidationGroupId] = (SELECT TOP(1) Id FROM [Billing].[OrderValidationRuleGroups] WHERE [Code] = {0}) 
WHERE [RuleCode] = {1}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            UpdateGroupCodeOrderValidationRuleGroupDetails(context.Connection, 1, 19);   // OrderValidationRuleGroups.Generic, OrderValidationRules.RequiredWhiteListedAdvertisementSpecified,
            UpdateGroupCodeOrderValidationRuleGroupDetails(context.Connection, 1, 21);   // OrderValidationRuleGroups.Generic, OrderValidationRules.SameAdvertisements
            DeleteOrderValidationIdColumn(context.Database);
        }

        private static void UpdateGroupCodeOrderValidationRuleGroupDetails(ServerConnection connection, int groupCode, int ruleCode)
        {
            connection.ExecuteNonQuery(string.Format(UpdateGroupCodeForGroupDetailStatement, groupCode, ruleCode));
        }

        private void DeleteOrderValidationIdColumn(Database database)
        {
            const string columnName = "OrderValidationId";
            if (database.Tables.Contains(ErmTableNames.OrderValidationResults.Name, ErmTableNames.OrderValidationResults.Schema))
            {
                var table = database.Tables[ErmTableNames.OrderValidationResults.Name, ErmTableNames.OrderValidationResults.Schema];
                if (table.Columns.Contains(columnName))
                {
                    var column = table.Columns[columnName];
                    column.Drop();
                }
            }
        }
    }
}