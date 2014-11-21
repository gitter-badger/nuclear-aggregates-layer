using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6509, "Добавление проверок, связанных с тематиками")]
    public class Migration6509 : TransactedMigration
    {
        private const string InsertStatementTemplate =
            @"INSERT INTO [Billing].[OrderValidationRuleGroupDetails] ([OrderValidationGroupId], [RuleCode]) VALUES ({0}, {1})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            AddRule(context.Connection, 1, 40); // DefaultThemeMustBeSpecified
            AddRule(context.Connection, 1, 41); // DefaultThemeMustContainOnlySelfAdv
            AddRule(context.Connection, 1, 42); // ThemePeriodOverlapsOrderPeriod
            AddRule(context.Connection, 1, 43); // ThemeCategories
            AddRule(context.Connection, 1, 44); // ThemePositionCount
        }

        private static void AddRule(ServerConnection connection, int groupCode, int ruleCode)
        {
            var command = string.Format(InsertStatementTemplate, groupCode, ruleCode);
            connection.ExecuteNonQuery(command);
        }
    }
}