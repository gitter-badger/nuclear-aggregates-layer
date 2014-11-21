using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5679, "Добавление проверки на дату окончания договора у профиля юр. лица")]
    public class Migration5679 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int groupId = 1;
            const int ruleId = 25;
            string script =
                string.Format(
                    "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId,RuleCode) VALUES ({0}, {1})",
                    groupId, ruleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
