using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5833, "Добавление проверки на форматирование текста РМ")]
    public class Migration5833 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int GroupId = 1;
            const int RuleId = 28;
            var script = string.Format(
                "INSERT INTO [Billing].[OrderValidationRuleGroupDetails] (OrderValidationGroupId,RuleCode) VALUES ({0}, {1})", GroupId, RuleId);
            context.Connection.ExecuteNonQuery(script);
        }
    }
}
