using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10023, "Включается проверка периода размещения заказа по тематике")]
    public sealed class Migration10023 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(
                @"IF NOT EXISTS(SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] where RuleCode = 42)
  INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId, RuleCode) VALUES (65, 1, 42)");
        }
    }
}
