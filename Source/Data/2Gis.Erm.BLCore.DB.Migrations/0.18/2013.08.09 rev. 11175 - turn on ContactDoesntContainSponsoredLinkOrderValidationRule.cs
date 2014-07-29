using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(11175, "Включаем проверку ContactDoesntContainSponsoredLinkOrderValidationRule")]
    public sealed class Migration11175 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int сontactDoesntContainSponsoredLinkOrderValidationRuleCode = 27;
            const int genericValidationGroup = 1;
            context.Connection.ExecuteNonQuery(
                string.Format(
                    @"IF NOT EXISTS(SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] WHERE RuleCode = {0})
INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId, RuleCode) VALUES (68, {1}, {0})",
                    сontactDoesntContainSponsoredLinkOrderValidationRuleCode,
                    genericValidationGroup));
        }
    }
}