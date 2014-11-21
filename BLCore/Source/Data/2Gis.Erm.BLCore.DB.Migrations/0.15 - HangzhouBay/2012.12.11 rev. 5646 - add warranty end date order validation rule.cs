using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5646, "Добавление проверки на дату окончания доверенности")]
    public class Migration5646 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int groupId = 1;
            const int ruleId = 24;
            string script =
                string.Format(
                    "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId,RuleCode) VALUES ({0}, {1})",
                    groupId, ruleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
