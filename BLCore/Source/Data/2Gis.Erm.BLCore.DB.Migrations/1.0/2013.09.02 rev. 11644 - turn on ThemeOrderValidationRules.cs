using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(11644, "Включаем проверки заказов по тематикам")]
    public sealed class Migration11644 : TransactedMigration
    {
        private const int DefaultThemeMustBeSpecifiedValidationRuleCode = 40;
        private const int DefaultThemeMustContainOnlySelfAdvValidationRuleCode = 41;
        private const int ThemeCategoriesValidationRuleCode = 43;
        private const int ThemePositionCountValidationRuleCode = 44;

        private const long Id1 = 177023616880214017;
        private const long Id2 = 177023616880214273;
        private const long Id3 = 177023616880214529;
        private const long Id4 = 177023616880214785;

        private const int GeneralGroupId = 1;

        protected override void ApplyOverride(IMigrationContext context)
        {
            TurnOnOrderValidationRule(context, DefaultThemeMustBeSpecifiedValidationRuleCode, GeneralGroupId, Id1);
            TurnOnOrderValidationRule(context, DefaultThemeMustContainOnlySelfAdvValidationRuleCode, GeneralGroupId, Id2);
            TurnOnOrderValidationRule(context, ThemeCategoriesValidationRuleCode, GeneralGroupId, Id3);
            TurnOnOrderValidationRule(context, ThemePositionCountValidationRuleCode, GeneralGroupId, Id4);
        }

        private void TurnOnOrderValidationRule(IMigrationContext context, int ruleCode, int groupCode, long id)
        {
            const string query = @"IF NOT EXISTS(SELECT * FROM [Billing].[OrderValidationRuleGroupDetails] WHERE OrderValidationGroupId = {0} AND RuleCode = {1})
                                   INSERT INTO  [Billing].[OrderValidationRuleGroupDetails] (Id, OrderValidationGroupId, RuleCode) VALUES ({2}, {0}, {1})";

            context.Database.ExecuteNonQuery(string.Format(query, groupCode, ruleCode, id));
        }
    }
}
