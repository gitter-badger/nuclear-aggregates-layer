using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3541, "Перемещение проверки на сопутствующие и запрещенные позиции в отдельную группу из Generic")]
    public class Migration3541 : TransactedMigration
    {
        private const string InsertGroupsStatement = @"INSERT INTO [Billing].[OrderValidationRuleGroups] ([Code]) VALUES ({0})";
        private const string UpdateGroupCodeForGroupDetailStatement = @"
UPDATE [Billing].[OrderValidationRuleGroupDetails] 
SET [OrderValidationGroupId] = (SELECT TOP(1) Id FROM [Billing].[OrderValidationRuleGroups] WHERE [Code] = {0}) 
WHERE [RuleCode] = {1}";


        protected override void ApplyOverride(IMigrationContext context)
        {
            InsertOrderValidationRuleGroup(context.Connection, 3);                      // OrderValidationRuleGroups.ADPositionsValidation
            UpdateGroupCodeOrderValidationRuleGroupDetails(context.Connection, 3, 6);   // OrderValidationRuleGroups.ADPositionsValidation, OrderValidationRules.AssociatedAndDeniedPositions
        }

        private static void InsertOrderValidationRuleGroup(ServerConnection connection, int groupCode)
        {
            connection.ExecuteNonQuery(string.Format(InsertGroupsStatement, groupCode));
        }

        private static void UpdateGroupCodeOrderValidationRuleGroupDetails(ServerConnection connection, int groupCode, int ruleCode)
        {
            connection.ExecuteNonQuery(string.Format(UpdateGroupCodeForGroupDetailStatement, groupCode, ruleCode));
        }
    }
}