using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5569, "Добавление проверки на наличие профиля у юр. лица")]
    public class Migration5569 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int groupId = 1;
            const int ruleId = 23;
            string script =
                string.Format(
                    "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId,RuleCode) VALUES ({0}, {1})",
                    groupId, ruleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
