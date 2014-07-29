using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(8098, "Включаем, отключенные во время сборки, проверки")]
    public sealed class Migration8098 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] where RuleCode = 27)
  INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId, RuleCode) VALUES (1, 27)

    IF NOT EXISTS(SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] where RuleCode = 36)
  INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId, RuleCode) VALUES (1, 36)");
        }
    }
}
